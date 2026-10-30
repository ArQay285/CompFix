using System;
using Microsoft.EntityFrameworkCore;

namespace CompFix_ConnectDb
{
	// Просто клас для тестів чи працює підключення до бази даних
	//TODO: Після тестів видалиш,якщо я забуду
public class AppDbContext2 
	{
        public int Id { get; set; }
        public int Number { get; set; }
		public string Sentens { get; set; }
		public AppDbContext2(int Number, string Sentens)
		{
			this.Number = Number;
			this.Sentens = Sentens;

        }
	}
}