using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Stations;
using RailTicketApp.RabbitMq;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationController : ControllerBase
    {
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;

        public StationController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
        }

        [HttpPost]
        public IActionResult CreateStation([FromBody] CreateStationCommand command)
        {
            _rabbitMqSender.SendMessage(command, _settings.StationQueueName, "CreateStationCommand");
            return Ok(new { Message = "CreateStationCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStation(int id)
        {
            var command = new DeleteStationCommand { StationId = id };
            _rabbitMqSender.SendMessage(command, _settings.StationQueueName, "DeleteStationCommand");
            return Ok(new { Message = "DeleteStationCommand has been sent to the queue" });
        }
    }
}
