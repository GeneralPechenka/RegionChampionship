using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenHeaderService _tokenHeaderService;
        private readonly ITokenProvider _jwtTokenProvider;

        public TokenValidationMiddleware(
            RequestDelegate next,
            ITokenHeaderService tokenHeaderService,
            ITokenProvider jwtTokenProvider)
        {
            _next = next;
            _tokenHeaderService = tokenHeaderService;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Пропускаем публичные эндпоинты
            if (ShouldSkipAuthentication(context))
            {
                await _next(context);
                return;
            }

            // 2. Получаем токен из заголовка через сервис
            var token = _tokenHeaderService.GetTokenFromRequest(context.Request);
            Console.WriteLine("TOKEN: "+token);
            // ДОБАВЬТЕ ДЛЯ ОТЛАДКИ:
            var logger = context.RequestServices.GetService<ILogger<TokenValidationMiddleware>>();

            // Логируем что пришло в заголовке
            var rawHeader = context.Request.Headers["X-Auth-Token"].FirstOrDefault();
            logger?.LogDebug($"Raw X-Auth-Token header: {rawHeader}");
            logger?.LogDebug($"Cleaned token: {token}");
            logger?.LogDebug($"Token length: {token?.Length ?? 0}");

            if (string.IsNullOrEmpty(token))
            {
                logger?.LogWarning("Token is null or empty after cleaning");
                await ReturnUnauthorizedAsync(context, "Отсутствует токен доступа. Авторизуйтесь ещё раз.");
                return;
            }

            // 3. Проверяем токен
            var validationResult = await _jwtTokenProvider.CheckExpireJwtTokenAsync(token);
            if (!validationResult)
            {
                logger?.LogWarning("Token validation failed");
                await ReturnUnauthorizedAsync(context, "Недействительный токен доступа. Авторизуйтесь ещё раз.");
                return;
            }

            // 4. Продолжаем выполнение
            await _next(context);
        }

        private bool ShouldSkipAuthentication(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            // Пропускаем auth эндпоинты
            if (path.Contains("auth", StringComparison.OrdinalIgnoreCase))
                return true;

            // Пропускаем health checks
            if (path.Contains("health", StringComparison.OrdinalIgnoreCase))
                return true;

            // Пропускаем swagger
            if (path.Contains("swagger", StringComparison.OrdinalIgnoreCase))
                return true;

            // Пропускаем favicon
            if (path.EndsWith("/favicon.ico", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private async Task ReturnUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(message);
        }
    }
}
