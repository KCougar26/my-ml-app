using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public StoreController(ShopContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // 1. Get all customers (for the "Select Customer" screen)
        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // 2. Get customer dashboard data (summary of their orders)
        [HttpGet("customers/{id}/dashboard")]
        public async Task<IActionResult> GetDashboard(int id)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == id)
                .ToListAsync();

            var summary = new
            {
                TotalOrders = orders.Count,
                TotalSpent = orders.Sum(o => o.OrderTotal),
                RecentOrders = orders.OrderByDescending(o => o.OrderDatetime).Take(5)
            };

            return Ok(summary);
        }

        // 3. Get order history for a specific customer
        [HttpGet("customers/{id}/orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderHistory(int id)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == id)
                .OrderByDescending(o => o.OrderDatetime)
                .ToListAsync();
        }

        // 3b. Admin: Get all orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderDatetime)
                .ToListAsync();
        }

        // 4. Place a new order
        [HttpPost("orders")]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            order.OrderDatetime = DateTime.Now;
            // Defaults to satisfy NOT NULL columns in Supabase schema
            order.PaymentMethod = string.IsNullOrWhiteSpace(order.PaymentMethod) ? "card" : order.PaymentMethod;
            order.DeviceType = string.IsNullOrWhiteSpace(order.DeviceType) ? "desktop" : order.DeviceType;
            order.IpCountry = string.IsNullOrWhiteSpace(order.IpCountry) ? "US" : order.IpCountry;

            if (order.OrderSubtotal <= 0)
            {
                order.OrderSubtotal = order.OrderTotal > 0 ? order.OrderTotal : 49.99;
            }
            if (order.ShippingFee < 0) order.ShippingFee = 0;
            if (order.TaxAmount < 0) order.TaxAmount = 0;
            if (order.OrderTotal <= 0)
            {
                order.OrderTotal = order.OrderSubtotal + order.ShippingFee + order.TaxAmount;
            }
            if (order.RiskScore < 0) order.RiskScore = 0;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // 5. Late Delivery Priority Queue (Top 50)
        [HttpGet("warehouse/priority-queue")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPriorityQueue()
        {
            return await _context.Orders
                .OrderByDescending(o => o.RiskScore)
                .Take(50)
                .ToListAsync();
        }

        // 6. Run Scoring (The ML Inference Trigger)
        [HttpPost("warehouse/run-scoring")]
        public async Task<IActionResult> RunScoring()
        {
            var orders = await _context.Orders.ToListAsync();
            var baseUrl = _configuration["Scoring:BaseUrl"] 
                          ?? _configuration["SCORING_BASE_URL"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return BadRequest(new { message = "Scoring service URL not configured." });
            }
            var client = _httpClientFactory.CreateClient();

            foreach (var order in orders)
            {
                var payload = new
                {
                    customerId = order.CustomerId,
                    orderTotal = order.OrderTotal,
                    paymentMethod = order.PaymentMethod,
                    deviceType = order.DeviceType,
                    ipCountry = order.IpCountry
                };

                try
                {
                    var response = await client.PostAsJsonAsync($"{baseUrl.TrimEnd('/')}/predict", payload);
                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    var result = await response.Content.ReadFromJsonAsync<ScoringResponse>();
                    if (result == null) continue;

                    // Convert probability (0-1) to risk_score (0-100)
                    order.RiskScore = Math.Round(result.probability * 100, 2);
                    order.IsFraud = result.is_fraud;
                }
                catch
                {
                    // If scoring fails for a record, skip it to keep the job moving
                    continue;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "ML Scoring job completed and priority queue refreshed." });
        }

        private sealed class ScoringResponse
        {
            public double probability { get; set; }
            public string risk_level { get; set; } = "LOW";
            public bool is_fraud { get; set; }
        }
    }
}
