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

        [HttpGet]
        public IActionResult GetStations([FromHeader(Name = "CorrelationId")] string correlationId)
        {
            _rabbitMqSender.SendMessage("", _settings.StationQueueName, "GetStations", correlationId);
            return Ok(new { Message = "GetStationsCommand has been sent to the queue" });
        }

        [HttpPost]
        public IActionResult CreateStation([FromBody] CreateStationCommand command, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            _rabbitMqSender.SendMessage(command, _settings.StationQueueName, "CreateStationCommand", correlationId);
            return Ok(new { Message = "CreateStationCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStation(int id, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            var command = new DeleteStationCommand { StationId = id };
            _rabbitMqSender.SendMessage(command, _settings.StationQueueName, "DeleteStationCommand", correlationId);
            return Ok(new { Message = "DeleteStationCommand has been sent to the queue" });
        }
    }
}
