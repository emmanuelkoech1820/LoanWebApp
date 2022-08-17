using Apps.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Apps.Data.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BankTransferRequest> BankTransferRequest { get; set; }
        public DbSet<LoanAccount> LoanAccount { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            var connection = Configuration.GetConnectionString("WebApiDatabase");
            options.UseSqlServer(connection);
            options.UseSqlServer(connection, b => b.MigrationsAssembly("WebApi"));
        }
    }
}