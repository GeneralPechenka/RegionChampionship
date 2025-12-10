namespace Domain.Exceptions
{
    // 15. Auth (для аутентификации)
    public class AuthException(string? message) : Exception(message)
    {
    }

}
