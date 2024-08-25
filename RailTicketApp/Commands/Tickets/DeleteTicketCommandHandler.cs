using RailTicketApp.Data;

namespace RailTicketApp.Commands.Tickets
{
    public class DeleteTicketCommandHandler
    {
        ILogger<DeleteTicketCommandHandler> _logger;
        private readonly DbContextClass _context;

        public DeleteTicketCommandHandler(DbContextClass context, ILogger<DeleteTicketCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(DeleteTicketCommand command)
        {
            _logger.LogInformation($"DeleteTicketCommandHandler: command {command} handled");
            var ticket = _context.Tickets.Find(command.TicketId);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
                _logger.LogInformation("DeleteTicketCommandHandler: ticket was deleted from database");
            }
            else
            {
                _logger.LogInformation("DeleteTicketCommandHandler: ticket was not found and was not deleted");
            }
        }
    }
}
