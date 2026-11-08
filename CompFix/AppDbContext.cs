using CompFix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

namespace CompFix
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Говоримо EF Core створити таблицю для нашої моделі
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Викликає базовий метод для налаштування таблиць Identity
            builder.Entity<Category>().HasData(
                new Category { id = 1, Name = "Комп'ютери" },
                new Category { id = 2, Name = "Ноутбуки" },
                new Category { id = 16, Name = "Ігрові комп'ютери" },
                new Category { id = 17, Name = "Ігрові ноутбуки" },
                new Category { id = 3, Name = "Смартфони" },
                new Category { id = 4, Name = "Планшети" },
                new Category { id = 5, Name = "Аксесуари" },
                new Category { id = 6, Name = "Ігрові приставки" },
                new Category { id = 7, Name = "Телевізори" },
                new Category { id = 8, Name = "Процесори" },
                new Category { id = 9, Name = "Відеокарти" },
                new Category { id = 10, Name = "Материнські плати" },
                new Category { id = 11, Name = "Оперативна пам'ять" },
                new Category { id = 12, Name = "Вібратори"},
                new Category { id = 13, Name = "Люди" },
                new Category { id = 14, Name = "Монітори" },
                new Category { id = 15, Name = "Побутова техніка"}

                );

            
        }
    }
}