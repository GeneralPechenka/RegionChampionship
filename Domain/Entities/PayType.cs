namespace Domain.Entities
{
    // 2. Типы оплаты
    public class PayType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // Наличные, Карта, QR
        public string Code { get; set; }

        public ICollection<VendingAndPayType> VendingMachines { get; set; }
    }
}
