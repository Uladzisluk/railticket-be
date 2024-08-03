using Microsoft.EntityFrameworkCore;
using RailTicketApp.Models;

namespace RailTicketApp.Data
{
    public class DbContextClass: DbContext
    {
        protected readonly IConfiguration Configuration;
        public DbContextClass(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Train> Trains { get; set; }
    }
}
