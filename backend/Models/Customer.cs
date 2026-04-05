namespace Backend.Models
{
    [Table("customers")]
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LoyaltyTier { get; set; }
    }
}