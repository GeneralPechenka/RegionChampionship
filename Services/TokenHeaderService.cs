using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Services
{
    public interface ITokenHeaderService
    {
        string? GetTokenFromRequest(HttpRequest request);
        void AddTokenToResponse(HttpResponse response, string token);
        void AddTokenToRequest(HttpRequestMessage request, string token);
    }

    public class TokenHeaderService : ITokenHeaderService
    {
        private const string TokenHeaderName = "X-Auth-Token";

        // Получить токен из входящего запроса
        public string? GetTokenFromRequest(HttpRequest request)
        {
            if (request.Headers.TryGetValue(TokenHeaderName, out var tokenHeader))
            {
                var token = tokenHeader.ToString();

                // ОЧИСТКА ТОКЕНА - ВАЖНО!
                token = CleanJwtToken(token);

                return token;
            }

            return null;
        }

        // Очистка JWT токена от мусора
        private string CleanJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return token;

            // 1. Удаляем пробелы
            token = token.Trim();

            // 2. Удаляем кавычки если есть
            if (token.StartsWith("\"") && token.EndsWith("\""))
            {
                token = token.Substring(1, token.Length - 2).Trim();
            }

            // 3. JWT должен содержать ровно 2 точки
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                // Пробуем найти действительный JWT внутри строки
                token = ExtractValidJwtFromString(token);
                parts = token.Split('.');
            }

            // 4. Проверяем каждую часть на валидность Base64Url
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = CleanBase64UrlString(parts[i]);
            }

            // 5. Собираем обратно
            return string.Join(".", parts);
        }

        // Извлечь валидный JWT из строки (на случай если есть мусор вокруг)
        private string ExtractValidJwtFromString(string input)
        {
            // Паттерн JWT: три группы Base64Url разделенные точками
            var jwtPattern = @"([A-Za-z0-9\-_=]+)\.([A-Za-z0-9\-_=]+)\.([A-Za-z0-9\-_=]+)";
            var match = Regex.Match(input, jwtPattern);

            if (match.Success && match.Groups.Count == 4)
            {
                return $"{match.Groups[1].Value}.{match.Groups[2].Value}.{match.Groups[3].Value}";
            }

            return input; // Возвращаем как есть, валидатор сам отклонит
        }

        // Очистка Base64Url строки
        private string CleanBase64UrlString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Удаляем все символы, не входящие в Base64Url алфавит
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    c == '-' || c == '_' || c == '=' || c == '.')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        // Добавить токен в исходящий ответ (если нужно)
        public void AddTokenToResponse(HttpResponse response, string token)
        {
            response.Headers.Append(TokenHeaderName, token);
        }

        // Добавить токен в исходящий HttpRequestMessage
        public void AddTokenToRequest(HttpRequestMessage request, string token)
        {
            request.Headers.Add(TokenHeaderName, token);
        }
    }
}