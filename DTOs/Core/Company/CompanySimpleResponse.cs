namespace DTOs.Core.Company
{
    public record CompanySimpleResponseDto(
        Guid Id,
        string Name,
        string? Email
    );
}
