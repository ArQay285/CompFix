using System;
using Microsoft.EntityFrameworkCore;

namespace CompFix.Models
{
	// Просто клас для тестів чи працює підключення до бази даних
	//TODO: Після тестів видалиш,якщо я забуду
	public class TestClass 
	{
		public int Id { get; set; }
		public int Number { get; set; }
		public string? Sentens { get; set; }
	}
}