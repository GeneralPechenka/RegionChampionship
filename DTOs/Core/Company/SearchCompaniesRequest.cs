namespace DTOs.Core.Company
{
    public record SearchCompaniesRequestDto(
        string? Name = null,
        string? Email = null,
        string? Phone = null,
        string? AddressContains = null,
        DateTime? CreatedFrom = null,
        DateTime? CreatedTo = null,
        bool? HasMachines = null,
        bool? HasEmployees = null,
        int PageNumber = 1,
        int PageSize = 20,
        string? SortBy = "Name",
        bool SortDescending = false
    );
}
