using Domain.Enums;

namespace Domain.Entities
{
    public class VendingMachine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Model { get; set; }
        public decimal TotalRevenue { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime CommissioningDate { get; set; }
        public DateTime? LastVerificationDate { get; set; }
        public int? VerificationIntervalMonths { get; set; }
        public DateTime? NextVerificationDate { get; set; }
        public int ResourceHours { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int MaintenanceDurationHours { get; set; }
        public MachineStatusEnum Status { get; set; }
        public DateTime? InventoryDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? ModemId { get; set; }
        public Guid? ProducerCountryId { get; set; }
        public Guid? LastVerificationEmployeeId { get; set; }

        public Company Company { get; set; }
        public Modem Modem { get; set; }
        public ProducerCountry ProducerCountry { get; set; }
        public Employee LastVerificationEmployee { get; set; }

        public ICollection<Sale> Sales { get; set; }
        public ICollection<Maintenance> Maintenances { get; set; }
        public ICollection<VendingAndPayType> PaymentTypes { get; set; }
        public ICollection<ProductStock> ProductStocks { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }

}
