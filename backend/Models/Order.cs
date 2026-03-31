using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDatetime { get; set; } = DateTime.Now;
        public double OrderTotal { get; set; }
        
        // ML Scoring property
        public double LateDeliveryProbability { get; set; } 
        
        // This links to the OrderItem class we fixed above
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}