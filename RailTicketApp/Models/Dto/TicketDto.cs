namespace RailTicketApp.Models.Dto
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string PassengerName { get; set; }
        public string TrainNumber { get; set; }
        public string DepartureStationName { get; set; }
        public string DepartureStationCity { get; set; }
        public string DepartureStationCountry { get; set; }
        public string ArrivalStationName { get; set; }
        public string ArrivalStationCity { get; set; }
        public string ArrivalStationCountry { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SeatNumber { get; set; }
        public string Status { get; set; }
    }
}
