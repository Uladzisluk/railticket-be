using RailTicketApp.Commands;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqTicketConsumer : IHostedService
    {
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqTicketConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _scopeFactory = scopeFactory;
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.TicketQueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var headers = ea.BasicProperties.Headers;
                var commandName = Encoding.UTF8.GetString((byte[])headers["command_name"]);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var createHandler = scope.ServiceProvider.GetRequiredService<CreateTicketCommandHandler>();
                    var deleteHandler = scope.ServiceProvider.GetRequiredService<DeleteTicketCommandHandler>();

                    if (commandName.Equals("CreateTicketCommand"))
                    {
                        var command = JsonConvert.DeserializeObject<CreateTicketCommand>(message);
                        createHandler.Handle(command);
                    }
                    else if (commandName.Equals("DeleteTicketCommand"))
                    {
                        var command = JsonConvert.DeserializeObject<DeleteTicketCommand>(message);
                        deleteHandler.Handle(command);
                    }
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _settings.TicketQueueName,
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
