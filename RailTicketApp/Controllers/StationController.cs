using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Stations;
using RailTicketApp.Models.Dto;
using RailTicketApp.RabbitMq;
using RailTicketApp.Services;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationController : ControllerBase
    {
        private readonly StationService _stationService;
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;

        public StationController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender, StationService stationService)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
            _stationService = stationService;
        }

        [HttpGet]
        public IActionResult GetStation()
        {
            var stations = _stationService.GetStations();

            if (stations == null || !stations.Any())
            {
                var emptyResponse = ResponseFactory.Ok("", 200, "No stations found");
                _rabbitMqSender.Dispose();
                return Ok(emptyResponse);
            }

            _rabbitMqSender.Dispose();
            return Ok(ResponseFactory.Ok(stations, 200, "Stations retrieved successfully"));
        }

        [HttpPost]
        public IActionResult CreateStation([FromBody] CreateStationCommand command, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            _rabbitMqSender.SendMessage(command, _settings.StationQueueName, "CreateStationCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "CreateStationCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStation(int id, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            var command = new DeleteStationCommand { StationId = id };
            _rabbitMqSender.SendMessage(command, _settings.StationQueueName, "DeleteStationCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "DeleteStationCommand has been sent to the queue" });
        }
    }
}
