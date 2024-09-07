using RailTicketApp.Data;
using RailTicketApp.Models.Dto;
using RailTicketApp.Models;

namespace RailTicketApp.Services
{
    public class RouteService
    {
        private readonly DbContextClass _dbContext;

        public RouteService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public List<RouteDto> GetRoutes()
        {
            List<Models.Route> routes = _dbContext.Routes.ToList();
            List<RouteDto> routeDtos = new List<RouteDto>();
            foreach (Models.Route rt in routes)
            {
                routeDtos.Add(new RouteDto
                {
                    Id = rt.Id,
                    TrainNumber = _dbContext.Trains.Find(rt.TrainId).Number,
                    DepartureStation = _dbContext.Stations.Find(rt.DepartureStationId).Name,
                    ArrivalStation = _dbContext.Stations.Find(rt.ArrivalStationId).Name,
                    DepartureTime = rt.DepartureTime,
                    ArrivalTime = rt.ArrivalTime
                });
            }
            return routeDtos;
        }
    }
}
