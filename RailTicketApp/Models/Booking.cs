namespace RailTicketApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateOnly BookingDate { get; set; }
        public int SeatNumber { get; set; }

        public Route Route { get; set; }
    }
}
