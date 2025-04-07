using FYP_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace FYP_Backend.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users -> Orders (1:M)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Orders -> OrderItems (1:M)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItems -> OrderItems (1:M)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Categories -> MenuItems (1:M)
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            // Order Entity Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.OrderType).IsRequired();
                entity.Property(e => e.PaymentStatus).IsRequired(); // ✅ NEW
            });

            // OrderItem Entity Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.SpecialInstructions).HasMaxLength(250); // ✅ NEW
            });

            // MenuItem Entity Configuration
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(300); // ✅ NEW
            });

            // Category Entity Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(300);
            });
        }
    }
}
