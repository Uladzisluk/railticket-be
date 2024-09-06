using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Dto;

namespace RailTicketApp.Commands.Trains
{
    public class CreateTrainCommandHandler
    {
        ILogger<CreateTrainCommandHandler> _logger;
        private readonly DbContextClass _context;

        public CreateTrainCommandHandler(DbContextClass context, ILogger<CreateTrainCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public TrainDto Handle(CreateTrainCommand command)
        {
            _logger.LogInformation($"CreateTrainCommandHandler: command {command} handled");
            var train = new Train
            {
                Name = command.Name,
                Number = command.Number,
            };

            _context.Trains.Add(train);
            _context.SaveChanges();
            _logger.LogInformation("CreateTrainCommandHandler: train was added to data base");

            return new TrainDto
            {
                Id = train.Id,
                Name = train.Name,
                Number = train.Number
            };
        }
    }
}
