using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqSenderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqSenderFactory> _logger;

        public RabbitMqSenderFactory(IServiceProvider serviceProvider, ILogger<RabbitMqSenderFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public RabbitMqSender CreateSender()
        {
            var options = _serviceProvider.GetRequiredService<IOptions<RabbitMqSettings>>();
            var logger = _serviceProvider.GetRequiredService<ILogger<RabbitMqSender>>();
            _logger.LogInformation("RabbitMqSenderFactory: new one sender created");

            return new RabbitMqSender(options, logger);
        }
    }
}
