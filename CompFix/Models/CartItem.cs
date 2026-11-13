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
        public int Discount { get; set; }
        public decimal Cart_DiscountedPrice { get; set; }
        public ItemCondition Cart_Condition { get; set; }
        public decimal TotalPrice =>(Discount > 0) ? Cart_DiscountedPrice * Quantity
            : Price * Quantity;
        public string ImageUrl { get; set; } = string.Empty;

    }
}
