namespace Backend.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Cost { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
