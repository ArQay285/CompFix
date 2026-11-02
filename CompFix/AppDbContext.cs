using CompFix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CompFix
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Говоримо EF Core створити таблицю для нашої моделі
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}