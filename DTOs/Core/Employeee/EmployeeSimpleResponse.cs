using Domain.Enums;

namespace DTOs.Core.Employeee
{
    public record EmployeeSimpleResponseDto(
        Guid Id,
        string FullName,
        string Email,
        EmployeeRoleEnum Role,
        string? CompanyName
    );
}
