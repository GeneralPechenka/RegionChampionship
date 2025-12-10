namespace Domain.Entities
{
    // 5. Компания/Франчайзи
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<VendingMachine> VendingMachines { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
