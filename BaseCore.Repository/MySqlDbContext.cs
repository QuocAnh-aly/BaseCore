using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using BaseCore.Entities;

namespace BaseCore.Repository
{
    /// <summary>
    /// Entity Framework Core DbContext for MySQL
    /// Used for teaching EF Core concepts (Bài 10)
    /// </summary>
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
        {
        }

        // Parameterless constructor for EF migrations
        public MySqlDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        // DbSet for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<GameAccount> GameAccounts { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).HasMaxLength(128).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Salt).HasColumnType("varbinary(256)");
                entity.Property(e => e.Contact).HasMaxLength(100);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.Image).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Created).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserType).HasDefaultValue(0);
                entity.HasIndex(e => e.UserName).IsUnique();
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                // Relationship with Category
                
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                entity.Property(e => e.ShippingAddress).HasMaxLength(500);

                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderDetail entity
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

                // Relationships
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AccessToken>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Token)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure GameAccount entity
            modelBuilder.Entity<GameAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.AccountPassword).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CategoryId).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired().HasDefaultValue("Available");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime2").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CreatedUser).HasMaxLength(100).IsRequired();

                // Additional marketplace fields
                entity.Property(e => e.GameName).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.IsSold).HasDefaultValue(false);
                entity.Property(e => e.SoldAt).HasColumnType("datetime2");

                entity.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Seller).WithMany().HasForeignKey(e => e.SellerId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Buyer).WithMany().HasForeignKey(e => e.BuyerId).OnDelete(DeleteBehavior.SetNull);
            });

            // Configure UserWallet entity
            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Balance).HasPrecision(18, 2).HasDefaultValue(0);
                entity.Property(e => e.UpdatedDateTime).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            // Configure TransactionHistory entity
            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.PaymentMethod).HasMaxLength(100);
                entity.Property(e => e.Created).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Type)
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(50);

                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.GameAccount).WithMany().HasForeignKey(e => e.GameAccountId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items" },
                new Category { Id = 3, Name = "Books", Description = "Books and publications" },
                new Category { Id = 4, Name = "Home & Garden", Description = "Home and garden products" },
                new Category { Id = 5, Name = "Sports", Description = "Sports equipment and accessories" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop Dell XPS 15", Price = 35000000, Stock = 10, CategoryId = 1, Description = "High-performance laptop", ImageUrl = "" },
                new Product { Id = 2, Name = "iPhone 15 Pro", Price = 28000000, Stock = 15, CategoryId = 1, Description = "Latest Apple smartphone", ImageUrl = "" },
                new Product { Id = 3, Name = "T-Shirt Cotton", Price = 250000, Stock = 100, CategoryId = 2, Description = "Comfortable cotton t-shirt", ImageUrl = "" },
                new Product { Id = 4, Name = "Programming Book", Price = 450000, Stock = 50, CategoryId = 3, Description = "Learn programming basics", ImageUrl = "" },
                new Product { Id = 5, Name = "Garden Tools Set", Price = 850000, Stock = 25, CategoryId = 4, Description = "Complete gardening toolkit", ImageUrl = "" }
            );

            // Note: Users are managed by AuthService (MongoDB)
            // User seed data is handled by MongoDbContext.SeedDataAsync()
        }
    }
}
