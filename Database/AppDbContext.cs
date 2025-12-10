using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<VendingMachine> VendingMachines { get; set; }
        public DbSet<PayType> PayTypes { get; set; }
        public DbSet<VendingAndPayType> VendingAndPayTypes { get; set; }
        public DbSet<ProducerCountry> ProducerCountries { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Modem> Modems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<EngineerTask> EngineerTasks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           // Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
