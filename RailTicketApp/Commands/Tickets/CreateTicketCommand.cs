namespace RailTicketApp.Commands.Tickets
{
    public class CreateTicketCommand
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SeatNumber { get; set; }
        public string Status { get; set; }
    }
}
