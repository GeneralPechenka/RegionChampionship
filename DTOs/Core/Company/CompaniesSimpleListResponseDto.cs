namespace DTOs.Core.Company
{
    public record CompaniesSimpleListResponseDto(
    List<CompanySimpleResponseDto> Companies,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);
}
