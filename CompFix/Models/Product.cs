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
        [StringLength(100,ErrorMessage ="Назва немає перевищувати більше 100 сиволів")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Вкажіть опис")]
        [StringLength(200, ErrorMessage = "Опис не має перевищувати 200 символів")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(0.01, 1000000, ErrorMessage = "Ціна має бути більше 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Вкажіть кількість")]
        [Range(0, 10000, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Quantity { get; set; }
        public ItemCondition Condition { get; set; } = ItemCondition.New;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        //  Поле знижки у відсотках (від 0 до 100)
        [Required(ErrorMessage = "Вкажіть знижку")]
        [Range(0, 100, ErrorMessage = "Знижка має бути від 0 до 100")]
        public int Discount { get; set; } = 0;
        // Обчислювана ціна з урахуванням знижки
        public decimal DiscountedPrice => Price * (1 - (decimal)Discount / 100);
        //Прапорець видимості(true = показувати покупцям, false = приховано)
        public bool IsAvailable { get; set; } = true;

    }
}
