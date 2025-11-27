using ClassLibrary;
using ClassLibrary.Model;
using Microsoft.EntityFrameworkCore;

namespace MadkassenRestAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Produkter> Produkter { get; set; }
        public DbSet<Kategori> Kategori { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Kategori entity configuration
            modelBuilder.Entity<Kategori>()
                .HasKey(k => k.CategoryId);

            modelBuilder.Entity<Kategori>()
                .Property(k => k.CategoryName)
                .HasColumnName("CategoryName")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Kategori>()
                .Property(k => k.Description)
                .HasColumnName("Description");

            // Produkter entity configuration
            modelBuilder.Entity<Produkter>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Produkter>()
                .Property(p => p.ProductName)
                .HasColumnName("ProductName")
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Produkter>()
                .Property(p => p.Description)
                .HasColumnName("Description");

            modelBuilder.Entity<Produkter>()
                .Property(p => p.AllergyType)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString() : null,
                    v => string.IsNullOrEmpty(v) ? (AllergyType?)null : Enum.Parse<AllergyType>(v)
                );

            modelBuilder.Entity<Produkter>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Produkter>()
                .Property(p => p.StockLevel)
                .HasColumnName("StockLevel");
            
            // Users entity configuration
            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Users>()
                .Property(u => u.UserName)
                .HasColumnName("UserName")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Users>()
                .Property(u => u.Email)
                .HasColumnName("Email")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Users>()
                .Property(u => u.PasswordHash)
                .HasColumnName("PasswordHash")
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Users>()
                .Property(u => u.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime")  // Changed from datetime2 to match database
                .IsRequired(true);

            modelBuilder.Entity<Users>()
                .Property(u => u.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime")  // Changed from datetime2 to match database
                .IsRequired(true);

            modelBuilder.Entity<Users>()
                .Property(u => u.Roles)
                .HasColumnName("Roles")
                .HasMaxLength(50)
                .IsRequired();

            // CartItem entity configuration
            modelBuilder.Entity<CartItem>()
                .HasKey(c => c.CartItemId);

            modelBuilder.Entity<CartItem>()
                .Property(c => c.CartItemId)
                .HasColumnName("CartItemId");

            modelBuilder.Entity<CartItem>()
                .Property(c => c.ProductId)
                .HasColumnName("ProductId")
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .Property(c => c.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .Property(c => c.Quantity)
                .HasColumnName("Quantity")
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .Property(c => c.AddedAt)
                .HasColumnName("AddedAt")
                .HasColumnType("datetime")  // Configure as datetime to match database
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .Property(c => c.ExpirationTime)
                .HasColumnName("ExpirationTime")
                .HasColumnType("datetime")  // Configure as datetime to match database
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Produkter)
                .WithMany()
                .HasForeignKey(c => c.ProductId);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Users)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            // Order entity configuration
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderId)
                .HasColumnName("OrderId")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Order>()
                .Property(o => o.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasColumnName("OrderDate")
                .HasColumnType("datetime")  // Changed from datetime2 to match database
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasColumnName("OrderStatus")
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Define the relationship between Orders and OrderItems (1-to-many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem entity configuration
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemId);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.OrderItemId)
                .HasColumnName("OrderItemId")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.OrderId)
                .HasColumnName("OrderId")
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.ProductId)
                .HasColumnName("ProductId")
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Quantity)
                .HasColumnName("Quantity")
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Produkter)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Users)
                .WithMany()
                .HasForeignKey(o => o.UserId);
        }
    }
}
