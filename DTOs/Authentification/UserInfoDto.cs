namespace DTOs.Authentification
{
    public record UserInfoDto(
        string Email,
        string FullName,
        string RoleName,
        string? CompanyName = null,
        string? Phone = null);
}
