using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return tokenHeader.ToString();
            }

            return null;
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
