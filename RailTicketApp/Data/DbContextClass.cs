using Microsoft.EntityFrameworkCore;
using RailTicketApp.Models;
using System.Diagnostics;

namespace RailTicketApp.Data
{
    public class DbContextClass : DbContext
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
        public DbSet<User> Users { get; set; }
    }
}
