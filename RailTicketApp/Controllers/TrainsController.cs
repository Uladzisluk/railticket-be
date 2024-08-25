﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands;
using RailTicketApp.Commands.Trains;
using RailTicketApp.RabbitMq;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainsController : ControllerBase
    {
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;

        public TrainsController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
        }

        [HttpPost]
        public IActionResult CreateTrain([FromBody] CreateTrainCommand command)
        {
            _rabbitMqSender.SendMessage(command, _settings.TrainQueueName, "CreateTrainCommand");
            return Ok(new { Message = "CreateTrainCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTrain(int id)
        {
            var command = new DeleteTrainCommand { TrainId = id };
            _rabbitMqSender.SendMessage(command, _settings.TrainQueueName, "DeleteTrainCommand");
            return Ok(new { Message = "DeleteTrainCommand has been sent to the queue" });
        }
    }
}
