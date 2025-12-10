namespace Domain.Entities
{
    // 8. Товары
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int MinStock { get; set; }
        public decimal SalesTendency { get; set; } // Средние продажи в день
        public string Category { get; set; }

        public ICollection<ProductStock> Stocks { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
