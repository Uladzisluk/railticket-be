using RailTicketApp.Data;
namespace RailTicketApp.Commands.Trains
{
    public class DeleteTrainCommandHandler
    {
        ILogger<DeleteTrainCommandHandler> _logger;
        private readonly DbContextClass _context;

        public DeleteTrainCommandHandler(DbContextClass context, ILogger<DeleteTrainCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Handle(DeleteTrainCommand command)
        {
            _logger.LogInformation($"DeleteTrainCommandHandler: command {command} handled");
            var train = _context.Trains.Find(command.TrainId);
            if (train != null)
            {
                _context.Trains.Remove(train);
                _context.SaveChanges();
                _logger.LogInformation("DeleteTrainCommandHandler: train was deleted from data base");
            }
            else {
                _logger.LogInformation("DeleteTrainCommandHandler: train was not found and was not deleted");
            }

        }
    }
}
