namespace RailTicketApp.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendTrainMessage < T >(T message);
    }
}
