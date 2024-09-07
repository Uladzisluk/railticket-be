using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqSender
    {
        private readonly ILogger<RabbitMqSender> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqSender(IOptions<RabbitMqSettings> options, ILogger<RabbitMqSender> logger)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger = logger;
        }

        public void SendMessage(object message, string queueName, string commandName, string correlationId)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            IBasicProperties props = _channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "command_name", commandName },
            };
            props.CorrelationId = correlationId;

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: props,
                                  body: body);
            _logger.LogInformation($"RabbitMqSender: message '{jsonMessage}' with header '{commandName}' was sent");
            Dispose();
        }

        private void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
