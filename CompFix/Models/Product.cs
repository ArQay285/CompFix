using System;
using System.ComponentModel.DataAnnotations;
namespace CompFix.Models
{
    public enum ItemCondition
    {
        New = 0,// Новий
        Used = 1,// Б/Y
        Refurbished = 2 // Відновлений
    }
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введіть будь ласка текст")]
        [StringLength(100,ErrorMessage ="Назва немає перевищувати більше 100 слів")]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(0.01, 1000000, ErrorMessage = "Ціна має бути більше 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Вкажіть кількість")]
        [Range(0, 10000, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Quantity { get; set; }
        public ItemCondition Condition { get; set; } = ItemCondition.New;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
