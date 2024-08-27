namespace RailTicketApp.RabbitMq
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; }
        public string TicketQueueName { get; set; }
        public string TrainQueueName { get; set; }
        public string StationQueueName { get; set; }
        public string RouteQueueName { get; set; }
    }
}
