using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace RailTicketApp.RabbitMq
{
    public class RabbitMqSenderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqSenderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public RabbitMqSender CreateSender()
        {
            var options = _serviceProvider.GetRequiredService<IOptions<RabbitMqSettings>>();
            var logger = _serviceProvider.GetRequiredService<ILogger<RabbitMqSender>>();

            return new RabbitMqSender(options, logger);
        }
    }
}
