using Domain.Enums;

namespace Domain.Entities
{
    // 10. Продажи
    public class Sale
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public DateTime SaleDate { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }

        public Guid VendingMachineId { get; set; }
        public Guid ProductId { get; set; }

        public VendingMachine VendingMachine { get; set; }
        public Product Product { get; set; }
    }
}
