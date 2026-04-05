using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDatetime { get; set; } = DateTime.Now;

        public string? BillingZip { get; set; }
        public string? ShippingZip { get; set; }
        public string? ShippingState { get; set; }
        public string PaymentMethod { get; set; } = "card";
        public string DeviceType { get; set; } = "desktop";
        public string IpCountry { get; set; } = "US";
        public bool PromoUsed { get; set; } = false;
        public string? PromoCode { get; set; }

        public double OrderSubtotal { get; set; }
        public double ShippingFee { get; set; }
        public double TaxAmount { get; set; }
        public double OrderTotal { get; set; }

        // Labels / targets
        public double RiskScore { get; set; } = 0; // 0-100
        public bool IsFraud { get; set; } = false;

        // This links to the OrderItem class
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
