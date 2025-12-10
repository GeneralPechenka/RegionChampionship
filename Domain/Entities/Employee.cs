using Domain.Enums;

namespace Domain.Entities
{
    // 6. Сотрудники
    public class Employee
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public EmployeeRoleEnum Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<VendingMachine> VerifiedMachines { get; set; }
        public ICollection<Maintenance> Maintenances { get; set; }
    }
}
