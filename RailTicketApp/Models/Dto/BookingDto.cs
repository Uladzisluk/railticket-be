namespace RailTicketApp.Models.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateOnly BookingDate { get; set; }
        public int SeatNumber { get; set; }
    }
}
