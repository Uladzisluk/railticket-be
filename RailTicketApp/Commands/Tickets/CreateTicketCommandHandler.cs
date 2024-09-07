using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.Commands.Tickets
{
    public class CreateTicketCommandHandler
    {
        ILogger<CreateTicketCommandHandler> _logger;
        private readonly DbContextClass _context;

        public CreateTicketCommandHandler(DbContextClass context, ILogger<CreateTicketCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public TicketDto Handle(CreateTicketCommand command)
        {
            _logger.LogInformation($"CreateTicketCommandHandler: command {command} handled");
            var ticket = new Ticket
            {
                UserId = command.UserId,
                RouteId = command.RouteId,
                PurchaseDate = command.PurchaseDate,
                SeatNumber = command.SeatNumber,
                Status = command.Status
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            _logger.LogInformation("CreateTicketCommandHandler: ticket was added to data base");

            Models.Route route = _context.Routes.Find(ticket.RouteId);
            return new TicketDto
            {
                Id = ticket.Id,
                PassengerName = _context.Users.Find(ticket.UserId).Name,
                TrainNumber = _context.Trains.Find(route.TrainId).Number,
                DepartureStationName = _context.Stations.Find(route.DepartureStationId).Name,
                DepartureStationCity = _context.Stations.Find(route.DepartureStationId).City,
                DepartureStationCountry = _context.Stations.Find(route.DepartureStationId).Country,
                ArrivalStationName = _context.Stations.Find(route.ArrivalStationId).Name,
                ArrivalStationCity = _context.Stations.Find(route.ArrivalStationId).City,
                ArrivalStationCountry = _context.Stations.Find(route.ArrivalStationId).Country,
                DepartureTime = route.DepartureTime,
                ArrivalTime = route.ArrivalTime,
                PurchaseDate = ticket.PurchaseDate,
                SeatNumber = ticket.SeatNumber,
                Status = ticket.Status
            };
        }
    }
}
