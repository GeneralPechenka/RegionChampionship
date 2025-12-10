namespace Domain.Entities
{
    // 4. Страна производителя
    public class ProducerCountry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } // RU, CN, etc.

        public ICollection<VendingMachine> VendingMachines { get; set; }
    }
}
