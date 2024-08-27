using RailTicketApp.Data;
using RailTicketApp.Models;

namespace RailTicketApp.Commands.Tickets
{
    public class CreateTicketCommandHandler
    {
        ILogger<CreateTicketCommandHandler> _logger;
        private readonly DbContextClass _context;

        public CreateTicketCommandHandler(DbContextClass context, ILogger<CreateTicketCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(CreateTicketCommand command)
        {
            _logger.LogInformation($"CreateTicketCommandHandler: command {command} handled");
            var ticket = new Ticket
            {
                UserId = command.UserId,
                RouteId = command.RouteId,
                PurchaseDate = command.PurchaseDate,
                SeatNumber = command.SeatNumber,
                Status = command.Status
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            _logger.LogInformation("CreateTicketCommandHandler: ticket was added to data base");
        }
    }
}
