namespace RailTicketApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SeatNumber { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
        public Route Route { get; set; }
    }
}
