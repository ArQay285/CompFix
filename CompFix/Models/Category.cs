namespace CompFix.Models
{
    public class Category
    {
        public int id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();

    }
}
