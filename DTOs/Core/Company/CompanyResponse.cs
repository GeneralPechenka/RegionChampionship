namespace DTOs.Core.Company
{
    public record CompanyResponseDto(
       Guid Id,
       string Name,
       string? Address,
       string? Phone,
       string? Email,
       DateTime CreatedAt,
       int TotalMachines,
       int TotalEmployees
   );
}
