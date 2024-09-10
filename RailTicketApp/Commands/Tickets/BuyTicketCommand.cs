namespace RailTicketApp.Commands.Tickets
{
    public class BuyTicketCommand
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public int SeatNumber { get; set; }
        public DateOnly Date {  get; set; }
    }
}
