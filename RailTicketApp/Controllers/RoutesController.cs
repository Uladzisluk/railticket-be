﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Routes;
using RailTicketApp.RabbitMq;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;

        public RoutesController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
        }

        [HttpPost]
        public IActionResult CreateRoute([FromBody] CreateRouteCommand command)
        {
            _rabbitMqSender.SendMessage(command, _settings.RouteQueueName, "CreateRouteCommand");
            return Ok(new { Message = "CreateRouteCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoute(int id)
        {
            var command = new DeleteRouteCommand { RouteId = id };
            _rabbitMqSender.SendMessage(command, _settings.RouteQueueName, "DeleteRouteCommand");
            return Ok(new { Message = "DeleteRouteCommand has been sent to the queue" });
        }
    }
}
