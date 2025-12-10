namespace DTOs.Core.Company
{
    public record CompaniesListResponseDto(
        List<CompanyResponseDto> Companies,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages
    );
}
