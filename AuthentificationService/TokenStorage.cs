namespace AuthentificationService
{
    public class TokenStorage
    {
        // Храним действительные токены: email -> token
        public Dictionary<string, string> ValidTokens { get; set; } = new();

        // Черный список токенов (завершённые и пр.)
        public HashSet<string> RevokedTokens { get; set; } = new();
    }
}
