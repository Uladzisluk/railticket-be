namespace RailTicketApp.Models
{
    public class TrainSeat
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public int SeatNumber { get; set; }
        public bool IsAvailable { get; set; }

        public Train train { get; set; }
    }
}
