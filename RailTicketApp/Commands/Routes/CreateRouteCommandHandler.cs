using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.Commands.Routes
{
    public class CreateRouteCommandHandler
    {
        ILogger<CreateRouteCommandHandler> _logger;
        private readonly DbContextClass _context;

        public CreateRouteCommandHandler(DbContextClass context, ILogger<CreateRouteCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public RouteDto Handle(CreateRouteCommand command)
        {
            _logger.LogInformation($"CreatRouteCommandHandler: command {command} handled");
            var route = new Models.Route
            {
                TrainId = command.TrainId,
                DepartureStationId = command.DepartureStationId,
                ArrivalStationId = command.ArrivalStationId,
                DepartureTime = command.DepartureTime,
                ArrivalTime = command.ArrivalTime,
                Train = _context.Trains.Find(command.TrainId),
                DepartureStation = _context.Stations.Find(command.DepartureStationId),
                ArrivalStation = _context.Stations.Find(command.ArrivalStationId)
            };

            _context.Routes.Add(route);
            _context.SaveChanges();
            _logger.LogInformation("CreateRouteCommandHandler: route was added to data base");
            return new RouteDto
            {
                Id = route.Id,
                TrainNumber = _context.Trains.Find(route.TrainId).Number,
                DepartureStation = _context.Stations.Find(route.DepartureStationId).Name,
                ArrivalStation = _context.Stations.Find(route.ArrivalStationId).Name,
                DepartureTime = route.DepartureTime,
                ArrivalTime = route.ArrivalTime
            };
        }
    }
}
