using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Dto;

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
                TrainId = route.TrainId,
                DepartureStationId = route.DepartureStationId,
                ArrivalStationId=route.ArrivalStationId,
                DepartureTime = route.DepartureTime,
                ArrivalTime = route.ArrivalTime
            };
        }
    }
}
