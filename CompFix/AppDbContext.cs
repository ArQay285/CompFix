using CompFix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


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
                new Category { id = 16, Name = "Мишки" },
                new Category { id = 17, Name = "Клавіатури" },
                new Category { id = 3, Name = "Смартфони" },
                new Category { id = 4, Name = "Планшети" },
                new Category { id = 5, Name = "Коврики для мишки" },
                new Category { id = 6, Name = "Ігрові приставки" },
                new Category { id = 7, Name = "Материнські плати" },
                new Category { id = 8, Name = "Процесори" },
                new Category { id = 9, Name = "Відеокарти" },
                new Category { id = 10, Name = "Блоки живлення" },
                new Category { id = 11, Name = "Оперативна пам'ять" },
                new Category { id = 12, Name = "Жорсткі диски" },
                new Category { id = 13, Name = "Корпуси для комп'ютерів" },
                new Category { id = 14, Name = "Охолодження" },
                new Category { id = 15, Name = "Монітори" }

                );

            
        }
    }
}