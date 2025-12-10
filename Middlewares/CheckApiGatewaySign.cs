using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Middlewares
{
    public class CheckApiGatewaySign(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {

            var signed = context.Request.Headers["ApiGateway"];

            if (signed.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Доступен доступ только через API Gateway");
                return;
            }
            await next(context);
        }
    }
}
