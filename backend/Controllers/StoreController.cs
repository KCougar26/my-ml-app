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

        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [HttpGet("customers/{id}/dashboard")]
        public async Task<IActionResult> GetDashboard(int id)
        {
            // Updated to match your CustomerId naming
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

        [HttpGet("warehouse/priority-queue")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPriorityQueue()
        {
            return await _context.Orders
                .OrderByDescending(o => o.RiskScore)
                .Take(50)
                .ToListAsync();
        }

        [HttpPost("run-scoring")]
        public async Task<IActionResult> RunScoring()
        {
            // 1. FETCH THE ORDERS (Fixed the missing variable)
            var orders = await _context.Orders.ToListAsync();

            var baseUrl = _configuration["Scoring:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return BadRequest(new { error = "ML URL is missing. Check Render Env Vars for 'Scoring__BaseUrl'" });
            }

            var client = _httpClientFactory.CreateClient();

            foreach (var order in orders)
            {
                var payload = new
                {
                    // Sending as snake_case for Python FastAPI
                    order_total = order.OrderTotal,
                    payment_method = order.PaymentMethod,
                    device_type = order.DeviceType,
                    ip_country = order.IpCountry
                };

                try
                {
                    var requestUrl = $"{baseUrl.TrimEnd('/')}/predict";
                    var response = await client.PostAsJsonAsync(requestUrl, payload);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ScoringResponse>();
                        if (result != null)
                        {
                            // Multiply by 100 to make it a "Score" out of 100
                            order.RiskScore = Math.Round(result.probability * 100, 2);
                            order.IsFraud = result.is_fraud;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Logs error to Render but keeps the loop moving
                    Console.WriteLine($"Error scoring order {order.OrderId}: {ex.Message}");
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