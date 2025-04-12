using FYP_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace FYP_Backend.Context
{
    // This class represents the EF Core database context for the application
    public class AppDbContext : DbContext
    {
        // Constructor that receives configuration options (like connection string)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define database tables (DbSets) corresponding to each model
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Fluent API configuration to customize model relationships and properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One User has many Orders, but deleting a user does NOT delete their orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Order has many OrderItems; deleting an order deletes its items
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // One MenuItem can appear in many OrderItems; deletion is restricted
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Category has many MenuItems; deleting a category doesn't delete items
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User entity fields
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);           // Email is required and max length is 100
                entity.Property(e => e.Role).IsRequired();                              // Role is required
                entity.Property(e => e.FirstName).HasMaxLength(50);                     // FirstName max length is 50
                entity.Property(e => e.LastName).HasMaxLength(50);                      // LastName max length is 50
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);                   // PhoneNumber max length is 20
                entity.Property(e => e.StudentId).IsRequired(false);                    // StudentId is optional (nullable)
                entity.HasIndex(e => e.StudentId).IsUnique();                           // StudentId must be unique if present
            });

            // Configure Order entity fields
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);      // Order number is required, max length 50
                entity.Property(e => e.Status).IsRequired();                            // Status is required (e.g. Pending, Completed)
                entity.Property(e => e.OrderType).IsRequired();                         // OrderType is required (Pickup or Delivery)
                entity.Property(e => e.PaymentStatus).IsRequired();                     // PaymentStatus is required (Unpaid, Succeeded, etc.)
            });

            // Configure OrderItem entity fields
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.SpecialInstructions).HasMaxLength(250);          // Optional instructions, max length 250
            });

            // Configure MenuItem entity fields
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);            // Name is required, max length 100
                entity.Property(e => e.Description).HasMaxLength(300);                  // Optional description, max length 300
            });

            // Configure Category entity fields
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);            // Name is required, max length 100
                entity.Property(e => e.Description).HasMaxLength(300);                  // Optional description, max length 300
            });
        }
    }
}
