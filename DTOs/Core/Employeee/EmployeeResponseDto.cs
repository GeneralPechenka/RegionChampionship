using Domain.Enums;

namespace DTOs.Core.Employeee
{
    public record EmployeeResponseDto(
    Guid Id,
    string FullName,
    string Email,
    EmployeeRoleEnum Role,
    DateTime CreatedAt,
    Guid? CompanyId,
    string? CompanyName,
    int VerifiedMachinesCount,
    int MaintenancesCount
);
}
