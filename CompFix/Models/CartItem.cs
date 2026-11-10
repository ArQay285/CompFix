namespace CompFix.Models
{
    // Клас для представлення елементу який в кошику ну як в стімі
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty; // Унікальний номер кошика (з Cookie)
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;


    }
}
