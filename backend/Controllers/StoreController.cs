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

        // 1. Get all customers
        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // 2. Get customer dashboard data
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

        // 4. Place a new order
        [HttpPost("orders")]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            order.OrderDatetime = DateTime.Now;
            order.PaymentMethod = string.IsNullOrWhiteSpace(order.PaymentMethod) ? "card" : order.PaymentMethod;
            order.DeviceType = string.IsNullOrWhiteSpace(order.DeviceType) ? "desktop" : order.DeviceType;
            order.IpCountry = string.IsNullOrWhiteSpace(order.IpCountry) ? "US" : order.IpCountry;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // 5. Priority Queue
        [HttpGet("warehouse/priority-queue")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPriorityQueue()
        {
            return await _context.Orders
                .OrderByDescending(o => o.RiskScore)
                .Take(50)
                .ToListAsync();
        }

        // 6. Run Scoring (THIS IS YOUR PREDICTION TRIGGER)
        // URL will be: https://store-backend-3sz6.onrender.com/api/Store/run-scoring
        [HttpPost("run-scoring")]
        public async Task<IActionResult> RunScoring()
        {
            var orders = await _context.Orders.ToListAsync();
            
            // Checks both appsettings and Render Env Vars
            var baseUrl = _configuration["SCORING_BASE_URL"] ?? _configuration["Scoring:BaseUrl"];
            
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return BadRequest(new { message = "Scoring service URL (SCORING_BASE_URL) is missing in Render settings." });
            }

            var client = _httpClientFactory.CreateClient();

            foreach (var order in orders)
            {
                var payload = new
                {
                    // Match these keys EXACTLY to what your Python FastAPI expects
                    order_total = order.OrderTotal,
                    payment_method = order.PaymentMethod,
                    device_type = order.DeviceType,
                    ip_country = order.IpCountry
                };

                try
                {
                    // Ensure the URL ends correctly
                    var requestUrl = $"{baseUrl.TrimEnd('/')}/predict";
                    var response = await client.PostAsJsonAsync(requestUrl, payload);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ScoringResponse>();
                        if (result != null)
                        {
                            order.RiskScore = Math.Round(result.probability * 100, 2);
                            order.IsFraud = result.is_fraud;
                        }
                    }
                }
                catch
                {
                    continue; 
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "ML Scoring job completed successfully." });
        }

        private sealed class ScoringResponse
        {
            public double probability { get; set; }
            public bool is_fraud { get; set; }
        }
    }
}