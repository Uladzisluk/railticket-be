namespace RailTicketApp.Models.Dto
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string TrainNumber { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
