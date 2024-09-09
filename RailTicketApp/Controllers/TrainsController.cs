using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Trains;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;
using RailTicketApp.RabbitMq;
using RailTicketApp.Services;

namespace RailTicketApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TrainsController : ControllerBase
    {
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;
        private readonly TrainService _trainService;

        public TrainsController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender, TrainService trainService)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
            _trainService = trainService;
        }

        [HttpGet]
        public IActionResult GetTrains()
        {
            var trains = _trainService.GetTrains();

            if (trains == null || !trains.Any())
            {
                var emptyResponse = ResponseFactory.Ok("", 200, "No trains found");
                _rabbitMqSender.Dispose();
                return Ok(emptyResponse);
            }

            _rabbitMqSender.Dispose();
            return Ok(ResponseFactory.Ok(trains, 200, "Trains retrieved successfully"));
        }

        [HttpPost]
        public IActionResult CreateTrain([FromBody] CreateTrainCommand command, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            _rabbitMqSender.SendMessage(command, _settings.TrainQueueName, "CreateTrainCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "CreateTrainCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTrain(int id, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            var command = new DeleteTrainCommand { TrainId = id };
            _rabbitMqSender.SendMessage(command, _settings.TrainQueueName, "DeleteTrainCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "DeleteTrainCommand has been sent to the queue" });
        }
    }
}
