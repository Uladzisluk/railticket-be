﻿namespace RailTicketApp.Models
{
    public class Route
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public int DepartureStationId { get; set; }
        public int ArrivalStationId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set;}

        public Train Train { get; set; }
        public Station DepartureStation { get; set; }
        public Station ArrivalStation { get; set; }
    }
}
