using RailTicketApp.Commands.Stations;
using RailTicketApp.Data;

namespace RailTicketApp.Commands.Routes
{
    public class DeleteRouteCommandHandler
    {
        ILogger<DeleteRouteCommandHandler> _logger;
        private readonly DbContextClass _context;

        public DeleteRouteCommandHandler(DbContextClass context, ILogger<DeleteRouteCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(DeleteRouteCommand command)
        {
            _logger.LogInformation($"DeleteRouteCommandHandler: command {command} handled");
            var route = _context.Routes.Find(command.RouteId);
            if (route != null)
            {
                _context.Routes.Remove(route);
                _context.SaveChanges();
                _logger.LogInformation("DeleteRouteCommandHandler: route was deleted from database");
            }
            else
            {
                _logger.LogInformation("DeleteRouteCommandHandler: route was not found and was not deleted");
            }
        }
    }
}
