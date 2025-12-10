namespace Domain.Entities
{
    public class VendingAndPayType
    {
        public Guid VendingMachineId { get; set; }
        public Guid PayTypeId { get; set; }

        public VendingMachine VendingMachine { get; set; }
        public PayType PayType { get; set; }
    }
}
