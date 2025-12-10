namespace Domain.Entities
{
    // 9. Остатки товаров в аппаратах
    public class ProductStock
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public DateTime LastRestock { get; set; }

        public Guid VendingMachineId { get; set; }
        public Guid ProductId { get; set; }

        public VendingMachine VendingMachine { get; set; }
        public Product Product { get; set; }
    }
}
