using CompFix_ConnectDb;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CompFix.ConnectDb
{
    public class AppDbContext: DbContext 
    {
        public DbSet<AppDbContext2> DataBaseInfo => Set<AppDbContext2>();//Підключення дял керування окремою таблицею
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOpionsBuilder) //Функція для налаштовування бази даних(вказування шляху і на що перекладати)
        { 
            dbContextOpionsBuilder.UseSqlServer("Server=localhost;Database=CompFixDb;Trusted_Connection=True;TrustServerCertificate=True");//Відповідає за підключення Sqlt І ЗА шлях
        }
    }
}
