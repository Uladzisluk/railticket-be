using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqSender
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqSender(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory() { HostName = _settings.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void SendMessage(object message, string queueName, string commandName)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            IBasicProperties props = _channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "command_name", commandName }
            };

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: props,
                                  body: body);
            Dispose();
        }

        private void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
