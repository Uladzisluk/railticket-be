using RailTicketApp.Data;
using RailTicketApp.Models.Dto;
using RailTicketApp.Models;

namespace RailTicketApp.Services
{
    public class TicketService
    {
        private readonly DbContextClass _dbContext;

        public TicketService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TicketDto> GetTickets()
        {
            List<Ticket> tickets = _dbContext.Tickets.ToList();
            List<TicketDto> ticketDtos = new List<TicketDto>();
            foreach (Ticket tk in tickets)
            {
                Models.Route route = _dbContext.Routes.Find(tk.RouteId);
                ticketDtos.Add(new TicketDto
                {
                    Id = tk.Id,
                    PassengerName = _dbContext.Users.Find(tk.UserId).Name,
                    TrainNumber = _dbContext.Trains.Find(route.TrainId).Number,
                    DepartureStationName = _dbContext.Stations.Find(route.DepartureStationId).Name,
                    DepartureStationCity = _dbContext.Stations.Find(route.DepartureStationId).City,
                    DepartureStationCountry = _dbContext.Stations.Find(route.DepartureStationId).Country,
                    ArrivalStationName = _dbContext.Stations.Find(route.ArrivalStationId).Name,
                    ArrivalStationCity = _dbContext.Stations.Find(route.ArrivalStationId).City,
                    ArrivalStationCountry = _dbContext.Stations.Find(route.ArrivalStationId).Country,
                    DepartureTime = route.DepartureTime,
                    ArrivalTime = route.ArrivalTime,
                    PurchaseDate = tk.PurchaseDate,
                    SeatNumber = tk.SeatNumber,
                    Status = tk.Status
                });;
            }
            return ticketDtos;
        }
    }
}
