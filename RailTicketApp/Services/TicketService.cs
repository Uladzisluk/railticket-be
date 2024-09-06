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
                ticketDtos.Add(new TicketDto
                {
                    Id = tk.Id,
                    UserId = tk.UserId,
                    RouteId = tk.RouteId,
                    PurchaseDate = tk.PurchaseDate,
                    SeatNumber = tk.SeatNumber,
                    Status = tk.Status
                });
            }
            return ticketDtos;
        }
    }
}
