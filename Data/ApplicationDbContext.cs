using Microsoft.EntityFrameworkCore;
using CrackersStore.API.Models;

namespace CrackersStore.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Shipping).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.ShippingAddress)
                    .WithMany()
                    .HasForeignKey("ShippingAddressId");
            });

            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Sky Thunder Rocket",
                    Category = "Rockets",
                    Price = 899,
                    OriginalPrice = 1299,
                    Image = "https://images.unsplash.com/photo-1529447738232-072452bb860f?w=400&q=80",
                    Rating = 4.5,
                    Reviews = 128,
                    Description = "Experience the ultimate aerial display with our Sky Thunder Rockets.",
                    InStock = true,
                    Features = new List<string> { "Height: 150 feet", "Duration: 15 seconds", "Safety tested" },
                    Safety = "Keep minimum 25 feet distance. Adult supervision required."
                },
                new Product
                {
                    Id = 2,
                    Name = "Golden Sparkler Premium",
                    Category = "Sparklers",
                    Price = 199,
                    OriginalPrice = 299,
                    Image = "https://images.unsplash.com/photo-1492674109156-e200e8c97546?w=400&q=80",
                    Rating = 4.8,
                    Reviews = 256,
                    Description = "Premium quality sparklers that burn bright and long.",
                    InStock = true,
                    Features = new List<string> { "Burn time: 60 seconds", "Pack of 10", "Smokeless" },
                    Safety = "Hold at arms length. Do not wave near face."
                }
            );
        }
    }
}
