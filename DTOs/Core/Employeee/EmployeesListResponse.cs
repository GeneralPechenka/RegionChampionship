namespace DTOs.Core.Employeee
{
    public record EmployeesListResponseDto(
        List<EmployeeResponseDto> Employees,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages
    );
}
