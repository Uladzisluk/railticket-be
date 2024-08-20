using RailTicketApp.Data;
using RailTicketApp.Models;

namespace RailTicketApp.Commands
{
    public class CreateTicketCommandHandler
    {
        private readonly DbContextClass _context;

        public CreateTicketCommandHandler(DbContextClass context)
        {
            _context = context;
        }

        public void Handle(CreateTicketCommand command)
        {
            var ticket = new Ticket
            {
                UserId = command.UserId,
                TrainId = command.TrainId,
                PurchaseDate = command.PurchaseDate,
                SeatNumber = command.SeatNumber,
                Status = command.Status
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }
    }
}
