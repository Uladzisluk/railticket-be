using Microsoft.EntityFrameworkCore;
using RailTicketApp.Models;

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
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TrainSeat> TrainSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
        }
    }
}
