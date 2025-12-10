
using AuthentificationService.Interfaces;
using AuthentificationService.Services;
using Database;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Middlewares;
using Services;
using Services.Options;
using Services.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AuthentificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("JwtOptions"));
            builder.Services.AddDbContext<AppDbContext>(
                    options => options.UseNpgsql(builder.Configuration
                      .GetConnectionString("PostgresConnection"))
                      .LogTo(Console.WriteLine, LogLevel.Warning));
            builder.Services.AddSingleton<TokenStorage>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IAuthService,AuthService>();
            builder.Services.AddScoped<UserValidator>();
            builder.Services.AddScoped<ITokenHeaderService, TokenHeaderService>();
            builder.Services.AddScoped<ITokenProvider,JwtProvider>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<CheckApiGatewaySign>();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
