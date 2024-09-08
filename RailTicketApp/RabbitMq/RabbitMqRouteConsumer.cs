using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RailTicketApp.Commands.Routes;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqRouteConsumer : IHostedService
    {
        private readonly RabbitMqSenderFactory _senderFactory;
        private readonly ILogger<RabbitMqRouteConsumer> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqRouteConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory, ILogger<RabbitMqRouteConsumer> logger,
            RabbitMqSenderFactory senderFactory)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _scopeFactory = scopeFactory;
            _logger = logger;
            InitializeRabbitMqListener();
            _senderFactory = senderFactory;
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.RouteQueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.QueueDeclare(queue: _settings.RouteQueueResponseName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
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
                _logger.LogInformation($"RabbitMqRouteConsumer: message '{message}' with header '{commandName}' recieved");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var createHandler = scope.ServiceProvider.GetRequiredService<CreateRouteCommandHandler>();
                    var deleteHandler = scope.ServiceProvider.GetRequiredService<DeleteRouteCommandHandler>();
                    try
                    {
                        if (commandName.Equals("CreateRouteCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<CreateRouteCommand>(message);
                            RouteDto routeDto = createHandler.Handle(command);

                            sender.SendMessage(ResponseFactory.Ok(routeDto, 200, "Route created"),
                                _settings.RouteQueueResponseName, commandName, correlationId);
                        }
                        else if (commandName.Equals("DeleteRouteCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<DeleteRouteCommand>(message);
                            deleteHandler.Handle(command);

                            sender.SendMessage(ResponseFactory.Ok("", 200, "Route deleted"),
                                _settings.RouteQueueResponseName, commandName, correlationId);
                        }
                    }   catch (Exception ex)
                    {
                        sender.SendMessage(ResponseFactory.Error("", 500, ex.GetType().Name, ex.Message),
                                    _settings.RouteQueueResponseName, commandName, correlationId);
                    }
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _settings.RouteQueueName,
                                  autoAck: false,
                                  consumer: consumer);
            _logger.LogInformation("RabbitMqRouteConsumer: consumer started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            _logger.LogInformation("RabbitMqRouteConsumer: consumer stopped async");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMqRouteConsumer: consumer stopped async");
        }
    }
}
