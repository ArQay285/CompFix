namespace CompFix.Models
{
    // блан заповення замовлення(вказування даних для доставки і покупки)
    public class Order
    {
        public int Id { get; set; }
       public string CustomerName { get; set; } = string.Empty;
        // Пошта покупця (сюди буде слатися підтвердження)
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        // Чи оплачено замовлення через банк (false = чекає оплати, true = оплачено)
        public bool IsPaid { get; set; } = false;
        // Список куплених товарів у цьому замовленні
        public List<OrderItem> OrderItems { get; set; } = new();


    }
}
