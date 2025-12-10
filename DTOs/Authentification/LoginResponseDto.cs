namespace DTOs.Authentification
{
    public record LoginResponseDto(string Token, DateTime Expires, UserInfoDto UserInfo);
}
