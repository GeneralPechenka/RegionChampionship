using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Services;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            builder.Services.AddSingleton<ITokenProvider, JwtProvider>();
            builder.Services.AddSingleton<ITokenHeaderService, TokenHeaderService>();
            builder.Services.AddOcelot(builder.Configuration);
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            var app = builder.Build();
            app.UseCors("AllowAll");
            app.UseMiddleware<TokenValidationMiddleware>();
            app.UseMiddleware<SignHttpsRequest>();


            app.UseOcelot();
            app.Run();
        }
    }
}
