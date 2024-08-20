using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands;
using RailTicketApp.RabbitMq;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;

        public TicketsController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
        }

        [HttpPost]
        public IActionResult CreateTicket([FromBody] CreateTicketCommand command)
        {
            _rabbitMqSender.SendMessage(command, _settings.TicketQueueName, "CreateTicketCommand");
            return Ok(new { Message = "CreateTicketCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var command = new DeleteTicketCommand { TicketId = id };
            _rabbitMqSender.SendMessage(command, _settings.TicketQueueName, "DeleteTicketCommand");
            return Ok(new { Message = "DeleteTicketCommand has been sent to the queue" });
        }
    }
}
