using Microsoft.EntityFrameworkCore;
using Backend.Models; // Ensure this matches your namespace

namespace Backend.Data
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map Table Names (Lowercase to match your SQLite tables)
            modelBuilder.Entity<Customer>().ToTable("customers");
            modelBuilder.Entity<Order>().ToTable("orders");
            modelBuilder.Entity<OrderItem>().ToTable("order_items");
            modelBuilder.Entity<Product>().ToTable("products");

            // Map Customer Columns
            modelBuilder.Entity<Customer>(entity => {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.FullName).HasColumnName("full_name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.LoyaltyTier).HasColumnName("loyalty_tier");
            });

            // Map Order Columns
            modelBuilder.Entity<Order>(entity => {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.OrderDatetime).HasColumnName("order_datetime");
                entity.Property(e => e.BillingZip).HasColumnName("billing_zip");
                entity.Property(e => e.ShippingZip).HasColumnName("shipping_zip");
                entity.Property(e => e.ShippingState).HasColumnName("shipping_state");
                entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
                entity.Property(e => e.DeviceType).HasColumnName("device_type");
                entity.Property(e => e.IpCountry).HasColumnName("ip_country");
                entity.Property(e => e.PromoUsed).HasColumnName("promo_used");
                entity.Property(e => e.PromoCode).HasColumnName("promo_code");
                entity.Property(e => e.OrderSubtotal).HasColumnName("order_subtotal");
                entity.Property(e => e.ShippingFee).HasColumnName("shipping_fee");
                entity.Property(e => e.TaxAmount).HasColumnName("tax_amount");
                entity.Property(e => e.OrderTotal).HasColumnName("order_total");
                entity.Property(e => e.RiskScore).HasColumnName("risk_score");
                entity.Property(e => e.IsFraud).HasColumnName("is_fraud");
            });

            // Map OrderItem Columns
            modelBuilder.Entity<OrderItem>(entity => {
                entity.HasKey(e => e.OrderItemId);
                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
                entity.Property(e => e.LineTotal).HasColumnName("line_total");
            });

            // Map Product Columns
            modelBuilder.Entity<Product>(entity => {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Sku).HasColumnName("sku");
                entity.Property(e => e.ProductName).HasColumnName("product_name");
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.Cost).HasColumnName("cost");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });
        }
    }
}
