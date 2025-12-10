namespace DTOs.General
{
    public record RequestDto<T>(string Token, T Value);
    public record ResponseDto<T>(string Token, T Value);
}
