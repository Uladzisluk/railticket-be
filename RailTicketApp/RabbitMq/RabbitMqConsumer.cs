using RailTicketApp.Commands;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqConsumer
    {
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitMqConsumer(RabbitMqSettings settings, IServiceScopeFactory scopeFactory)
        {
            _settings = settings;
            _scopeFactory = scopeFactory;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _settings.QueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var createHandler = scope.ServiceProvider.GetRequiredService<CreateTicketCommandHandler>();
                    var deleteHandler = scope.ServiceProvider.GetRequiredService<DeleteTicketCommandHandler>();

                    if (message.Contains("CreateTicketCommand"))
                    {
                        var command = JsonConvert.DeserializeObject<CreateTicketCommand>(message);
                        createHandler.Handle(command);
                    }
                    else if (message.Contains("DeleteTicketCommand"))
                    {
                        var command = JsonConvert.DeserializeObject<DeleteTicketCommand>(message);
                        deleteHandler.Handle(command);
                    }
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: _settings.QueueName,
                                  autoAck: false,
                                  consumer: consumer);
        }
    }
}
