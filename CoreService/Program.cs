
using Database;
using Middlewares;

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

            var app = builder.Build();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //    // ? Делай здесь, а не в конструкторе!
            //    dbContext.Database.EnsureCreated();

            //}

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
