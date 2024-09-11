using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.Services
{
    public class BookingService
    {
        private readonly DbContextClass _dbContext;

        public BookingService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public List<BookingDto> GetBookingDtos()
        {
            List<Booking> bookings = _dbContext.Bookings.ToList();
            List<BookingDto> bookingDtos = new List<BookingDto>();
            foreach (Booking bk in bookings)
            {
               bookingDtos.Add(new BookingDto
                {
                    Id = bk.Id,
                    RouteId = bk.RouteId,
                    BookingDate = bk.BookingDate,
                    SeatNumber = bk.SeatNumber
                });
            }
            return bookingDtos;
        }

        public Booking GetBooking(int id)
        {
            Booking booking = _dbContext.Bookings.Find(id);
            if (booking == null)
            {
                throw new NullReferenceException("There is no booking with id " + id + " in db");
            }
            return booking;
        }

        public List<BookingDto> GetBookingDtosByRouteId(int routeId)
        {
            List<Booking> bookings = _dbContext.Bookings.Where(b => b.RouteId == routeId).ToList();
            List<BookingDto> bookingDtos = new List<BookingDto>();
            foreach (Booking bk in bookings)
            {
                bookingDtos.Add(new BookingDto
                {
                    Id = bk.Id,
                    RouteId = bk.RouteId,
                    BookingDate = bk.BookingDate,
                    SeatNumber = bk.SeatNumber
                });
            }
            return bookingDtos;
        }
    }
}
