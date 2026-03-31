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
                entity.Property(e => e.OrderTotal).HasColumnName("order_total");
                
                // IMPORTANT: Since 'late_delivery_probability' isn't in your original shop.db yet,
                // we tell EF Core to ignore it for now so it doesn't crash.
                entity.Ignore(e => e.LateDeliveryProbability);
            });
        }
    }
}