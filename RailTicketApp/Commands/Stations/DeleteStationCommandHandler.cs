using RailTicketApp.Data;

namespace RailTicketApp.Commands.Stations
{
    public class DeleteStationCommandHandler
    {
        ILogger<DeleteStationCommandHandler> _logger;
        private readonly DbContextClass _context;

        public DeleteStationCommandHandler(DbContextClass context, ILogger<DeleteStationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(DeleteStationCommand command)
        {
            _logger.LogInformation($"DeleteStationCommandHandler: command {command} handled");
            var station = _context.Stations.Find(command.StationId);
            if (station != null)
            {
                _context.Stations.Remove(station);
                _context.SaveChanges();
                _logger.LogInformation("DeleteTStationCommandHandler: station was deleted from database");
            }
            else
            {
                _logger.LogInformation("DeleteStationCommandHandler: station was not found and was not deleted");
            }
        }
    }
}
