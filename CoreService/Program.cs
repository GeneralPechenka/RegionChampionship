
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

            var profiles = new Type[]{
                typeof(VendingMapProfile),
                typeof(MaintenanceProfile),
                typeof(PayTypeMapProfile)
            };
            builder.Services.AddAutoMapper(profiles);
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
