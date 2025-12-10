using Domain.Enums;

namespace Domain.Entities
{
    // 11. Обслуживание/ремонт
    public class Maintenance
    {
        public Guid Id { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string? WorkDescription { get; set; }
        public string Problems { get; set; }
        public MaintenanceTypeEnum Type { get; set; }

        public Guid VendingMachineId { get; set; }
        public Guid? EmployeeId { get; set; }

        public VendingMachine VendingMachine { get; set; }
        public Employee Employee { get; set; }
    }
}
