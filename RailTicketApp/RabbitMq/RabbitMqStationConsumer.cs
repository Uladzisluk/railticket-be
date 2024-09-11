using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RailTicketApp.Commands.Stations;
using System.Text;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqStationConsumer: IHostedService
    {
        private readonly RabbitMqSenderFactory _senderFactory;
        private readonly ILogger<RabbitMqStationConsumer> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqStationConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory, ILogger<RabbitMqStationConsumer> logger,
            RabbitMqSenderFactory senderFactory)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _scopeFactory = scopeFactory;
            _logger = logger;
            _senderFactory = senderFactory;
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.StationQueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.ExchangeDeclare(exchange: "station_exchange", type: ExchangeType.Fanout, durable: true);

            _channel.QueueDeclare(queue: _settings.StationQueueResponseName,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

            _channel.QueueBind(queue: _settings.StationQueueResponseName,
                               exchange: "station_exchange",
                               routingKey: "");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var sender = _senderFactory.CreateSender();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var correlationId = ea.BasicProperties.CorrelationId;
                var headers = ea.BasicProperties.Headers;
                var commandName = Encoding.UTF8.GetString((byte[])headers["command_name"]);
                _logger.LogInformation($"RabbitMqStationConsumer: message '{message}' with header '{commandName}' recieved");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var createHandler = scope.ServiceProvider.GetRequiredService<CreateStationCommandHandler>();
                    var deleteHandler = scope.ServiceProvider.GetRequiredService<DeleteStationCommandHandler>();
                    try
                    {
                        if (commandName.Equals("CreateStationCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<CreateStationCommand>(message);
                            StationDto stationDto = createHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok(stationDto, 200, "Operation successfull"),
                                "station_exchange", commandName, correlationId);
                        }
                        else if (commandName.Equals("DeleteStationCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<DeleteStationCommand>(message);
                            deleteHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok("", 200, "Station deleted"),
                                "station_exchange", commandName, correlationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        sender.SendMessageToExchange(ResponseFactory.Error("", 500, ex.GetType().Name, ex.Message),
                                    "station_exchange", commandName, correlationId);
                    }
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _settings.StationQueueName,
                                  autoAck: false,
                                  consumer: consumer);
            _logger.LogInformation("RabbitMqStationConsumer: consumer started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            _logger.LogInformation("RabbitMqStationConsumer: consumer stopped async");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMqStationConsumer: consumer stopped async");
        }
    }
}
