using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Tickets;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqTicketConsumer : IHostedService
    {
        private readonly RabbitMqSenderFactory _senderFactory;
        private readonly ILogger<RabbitMqTicketConsumer> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqTicketConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory, ILogger<RabbitMqTicketConsumer> logger,
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

            _channel.QueueDeclare(queue: _settings.TicketQueueName,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

            _channel.ExchangeDeclare(exchange: "ticket_exchange", type: ExchangeType.Fanout, durable: true);

            _channel.QueueDeclare(queue: _settings.TicketQueueResponseName,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

            _channel.QueueBind(queue: _settings.TicketQueueResponseName,
                               exchange: "ticket_exchange",
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
                _logger.LogInformation($"RabbitMqTicketConsumer: message '{message}' with header '{commandName}' recieved");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var createHandler = scope.ServiceProvider.GetRequiredService<CreateTicketCommandHandler>();
                    var deleteHandler = scope.ServiceProvider.GetRequiredService<DeleteTicketCommandHandler>();
                    var buyHandler = scope.ServiceProvider.GetRequiredService<BuyTicketCommandHandler>();
                    try
                    {
                        if (commandName.Equals("CreateTicketCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<CreateTicketCommand>(message);
                            TicketDto ticketDto = createHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok(ticketDto, 200, "Ticket created"),
                                "ticket_exchange", commandName, correlationId);
         
                        }
                        else if (commandName.Equals("DeleteTicketCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<DeleteTicketCommand>(message);
                            deleteHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok("", 200, "Ticket deleted"),
                                "ticket_exchange", commandName, correlationId);
                        }else if (commandName.Equals("BuyTicketCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<BuyTicketCommand>(message);
                            BookingDto bookingDto = buyHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok(bookingDto, 200, "Ticket bought"),
                                "ticket_exchange", commandName, correlationId);
                        }
                    }catch(Exception ex)
                    {
                        sender.SendMessageToExchange(ResponseFactory.Error("", 500, ex.GetType().Name, ex.Message),
                                    "ticket_exchange", commandName, correlationId);
                    }
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _settings.TicketQueueName,
                                  autoAck: false,
                                  consumer: consumer);
            _logger.LogInformation("RabbitMqTicketConsumer: consumer started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            _logger.LogInformation("RabbitMqTicketConsumer: consumer stopped async");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMqTicketConsumer: consumer stopped async");
        }
    }
}
