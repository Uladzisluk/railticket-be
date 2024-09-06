using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.Commands.Stations
{
    public class CreateStationCommandHandler
    {
        ILogger<CreateStationCommandHandler> _logger;
        private readonly DbContextClass _context;

        public CreateStationCommandHandler(DbContextClass context, ILogger<CreateStationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public StationDto Handle(CreateStationCommand command)
        {
            _logger.LogInformation($"CreateStationCommandHandler: command {command} handled");
            var station = new Station
            {
                Name = command.Name,
                City = command.City,
                Country = command.Country
            };

            _context.Stations.Add(station);
            _context.SaveChanges();
            _logger.LogInformation("CreateStationCommandHandler: station was added to data base");

            return new StationDto
            {
                Id = station.Id,
                Name = station.Name,
                City = station.City,
                Country = station.Country
            };
        }
    }
}
