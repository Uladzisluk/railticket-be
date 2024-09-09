namespace RailTicketApp.Commands.Routes
{
    public class CreateRouteCommand
    {
        public int TrainId { get; set; }
        public int DepartureStationId { get; set; }
        public int ArrivalStationId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
    }
}
