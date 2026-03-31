using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly ShopContext _context;

        public StoreController(ShopContext context)
        {
            _context = context;
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

        // 4. Place a new order
        [HttpPost("orders")]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            order.OrderDatetime = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // 5. Late Delivery Priority Queue (Top 50)
        [HttpGet("warehouse/priority-queue")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPriorityQueue()
        {
            return await _context.Orders
                .OrderByDescending(o => o.LateDeliveryProbability)
                .Take(50)
                .ToListAsync();
        }

        // 6. Run Scoring (The ML Inference Trigger)
        [HttpPost("warehouse/run-scoring")]
        public async Task<IActionResult> RunScoring()
        {
            var orders = await _context.Orders.ToListAsync();
            var random = new Random();

            foreach (var order in orders)
            {
                // Note: In Part 2, you'll replace this random number with your actual ML model logic
                order.LateDeliveryProbability = random.NextDouble();
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "ML Scoring job completed and priority queue refreshed." });
        }
    }
}