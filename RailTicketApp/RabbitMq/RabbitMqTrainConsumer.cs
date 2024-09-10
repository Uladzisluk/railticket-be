using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RailTicketApp.Commands.Trains;
using System.Text;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqTrainConsumer: IHostedService
    {
        private readonly RabbitMqSenderFactory _senderFactory;
        private readonly ILogger<RabbitMqTrainConsumer> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IModel _channel;
        private RabbitMqSender sender;

        public RabbitMqTrainConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory, ILogger<RabbitMqTrainConsumer> logger,
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

            _channel.QueueDeclare(queue: _settings.TrainQueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.ExchangeDeclare(exchange: "train_exchange", type: ExchangeType.Fanout, durable: true);

            _channel.QueueDeclare(queue: _settings.TrainQueueResponseName,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

            _channel.QueueBind(queue: _settings.TrainQueueResponseName,
                               exchange: "train_exchange",
                               routingKey: "");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            sender = _senderFactory.CreateSender();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var correlationId = ea.BasicProperties.CorrelationId;
                var headers = ea.BasicProperties.Headers;
                var commandName = Encoding.UTF8.GetString((byte[])headers["command_name"]);
                _logger.LogInformation($"RabbitMqTrainConsumer: message '{message}' with header '{commandName}' recieved");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var createHandler = scope.ServiceProvider.GetRequiredService<CreateTrainCommandHandler>();
                    var deleteHandler = scope.ServiceProvider.GetRequiredService<DeleteTrainCommandHandler>();
                    try
                    {
                        if (commandName.Equals("CreateTrainCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<CreateTrainCommand>(message);
                            TrainDto trainDto = createHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok(trainDto, 200, "Train created"),
                                "train_exchange", commandName, correlationId);
                        }
                        else if (commandName.Equals("DeleteTrainCommand"))
                        {
                            var command = JsonConvert.DeserializeObject<DeleteTrainCommand>(message);
                            deleteHandler.Handle(command);

                            sender.SendMessageToExchange(ResponseFactory.Ok("Train was deleted", 200, "Train deleted"),
                                "train_exchange", commandName, correlationId);
                        }
                    }catch(Exception ex)
                    {
                        sender.SendMessageToExchange(ResponseFactory.Error("", 500, ex.GetType().Name, ex.Message),
                                    "train_exchange", commandName, correlationId);
                    }
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _settings.TrainQueueName,
                                  autoAck: false,
                                  consumer: consumer);
            _logger.LogInformation("RabbitMqTrainConsumer: consumer started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            _logger.LogInformation("RabbitMqTrainConsumer: consumer stopped async");
            sender.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMqTrainConsumer: consumer disposed");
            sender.Dispose();
        }
    }
}
