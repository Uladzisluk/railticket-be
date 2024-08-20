namespace RailTicketApp.Models
{
    public class Route
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public int DepartureStationId { get; set; }
        public int ArrivalStationId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set;}

        public Train Train { get; set; }
        public Station DepartureStation { get; set; }
        public Station ArrivalStation { get; set; }
    }
}
