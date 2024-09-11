using RailTicketApp.Data;
using RailTicketApp.Models.Dto;
using RailTicketApp.Models;

namespace RailTicketApp.Commands.Tickets
{
    public class BuyTicketCommandHandler
    {
        ILogger<BuyTicketCommandHandler> _logger;
        private readonly DbContextClass _context;

        public BuyTicketCommandHandler(DbContextClass context, ILogger<BuyTicketCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public BookingDto Handle(BuyTicketCommand command)
        {
            _logger.LogInformation($"BuyTicketCommandHandler: command {command} handled");
            Booking booking = new Booking
            {
                RouteId = command.RouteId,
                BookingDate = command.Date,
                SeatNumber = command.SeatNumber,
                Route = _context.Routes.Find(command.RouteId)
            };
            Ticket ticket = new Ticket
            {
                UserId = command.UserId,
                RouteId = command.RouteId,
                PurchaseDate = DateTime.UtcNow,
                SeatNumber = command.SeatNumber.ToString(),
                Status = "Confirmed",
                User = _context.Users.Find(command.UserId),
                Route = _context.Routes.Find(command.RouteId)
            };

            _context.Bookings.Add(booking);
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            _logger.LogInformation("BuyTicketCommandHandler: ticket and booking were added to data base");

            return new BookingDto
            {
                Id = booking.Id,
                RouteId = booking.RouteId,
                BookingDate = booking.BookingDate,
                SeatNumber = booking.SeatNumber
            };
        }
    }
}
