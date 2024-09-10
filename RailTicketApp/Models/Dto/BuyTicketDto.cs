namespace RailTicketApp.Models.Dto
{
    public class BuyTicketDto
    {
        public int RouteId { get; set; }
        public int SeatNumber { get; set; }
        public DateOnly Date { get; set; }
    }
}
