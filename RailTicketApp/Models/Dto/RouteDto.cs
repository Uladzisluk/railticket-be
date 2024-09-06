namespace RailTicketApp.Models.Dto
{
    public class RouteDto
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public int DepartureStationId { get; set; }
        public int ArrivalStationId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
