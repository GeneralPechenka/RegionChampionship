namespace Domain.Entities
{
    // 7. Модемы
    public class Modem
    {
        public Guid Id { get; set; }
        public string Imei { get; set; }
        public string Provider { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ActivatedAt { get; set; }
        public bool IsActive { get; set; }

        public VendingMachine VendingMachine { get; set; }
    }
}
