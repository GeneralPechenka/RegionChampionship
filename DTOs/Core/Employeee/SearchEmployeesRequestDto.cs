using Domain.Enums;

namespace DTOs.Core.Employeee
{
    // Поиск сотрудников
    public record SearchEmployeesRequestDto(
        string? FullName = null,
        string? Email = null,
        EmployeeRoleEnum? Role = null,
        Guid? CompanyId = null,
        DateTime? CreatedFrom = null,
        DateTime? CreatedTo = null,
        bool? HasVerifiedMachines = null,
        bool? HasMaintenances = null,
        int PageNumber = 1,
        int PageSize = 20,
        string? SortBy = "FullName",
        bool SortDescending = false
    );
}
