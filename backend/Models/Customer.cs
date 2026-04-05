namespace Backend.Models
{
    public class Customer
    {
        // Change this from 'Id' to 'CustomerId' to match your Fluent API mapping
        public int CustomerId { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LoyaltyTier { get; set; }
    }
}