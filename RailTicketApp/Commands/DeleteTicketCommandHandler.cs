using RailTicketApp.Data;

namespace RailTicketApp.Commands
{
    public class DeleteTicketCommandHandler
    {
        private readonly DbContextClass _context;

        public DeleteTicketCommandHandler(DbContextClass context)
        {
            _context = context;
        }

        public void Handle(DeleteTicketCommand command)
        {
            var ticket = _context.Tickets.Find(command.TicketId);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
            }
        }
    }
}
