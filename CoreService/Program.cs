
using Database;
using DTOs.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using Middlewares;
using Services.Options;

namespace CoreService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Конфигурация (только JwtOptions если нужно для чего-то другого)
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("JwtOptions"));

            // Database
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseNpgsql(builder.Configuration
                    .GetConnectionString("PostgresConnection"))
                    .LogTo(Console.WriteLine, LogLevel.Warning)
                    );

            // AutoMapper
            var profiles = new Type[]{
                typeof(VendingMapProfile),
                typeof(MaintenanceProfile),
                typeof(PayTypeMapProfile)
            };

            // Исправьте регистрацию AutoMapper (проблема с версией)
            //builder.Services.AddAutoMapper(typeof(VendingMapProfile).Assembly, typeof(MaintenanceProfile).Assembly, typeof(PayTypeMapProfile).Assembly);
            //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // ИЛИ более явно
            // builder.Services.AddAutoMapper(profiles);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // ТОЛЬКО проверка подписи от API Gateway
            app.UseMiddleware<CheckApiGatewaySign>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
