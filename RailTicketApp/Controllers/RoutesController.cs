using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Routes;
using RailTicketApp.Models.Dto;
using RailTicketApp.RabbitMq;
using RailTicketApp.Services;

namespace RailTicketApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly RouteService _routeService;
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;

        public RoutesController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender, RouteService routeService)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
            _routeService = routeService;
        }

        [HttpGet]
        public IActionResult GetRoutes()
        {
            var routes = _routeService.GetRoutes();

            if (routes == null || !routes.Any())
            {
                var emptyResponse = ResponseFactory.Ok("", 200, "No routes found");
                _rabbitMqSender.Dispose();
                return Ok(emptyResponse);
            }

            _rabbitMqSender.Dispose();
            return Ok(ResponseFactory.Ok(routes, 200, "Routes retrieved successfully"));
        }

        [HttpPost]
        public IActionResult CreateRoute([FromBody] CreateRouteCommand command, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            _rabbitMqSender.SendMessage(command, _settings.RouteQueueName, "CreateRouteCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "CreateRouteCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoute(int id, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            var command = new DeleteRouteCommand { RouteId = id };
            _rabbitMqSender.SendMessage(command, _settings.RouteQueueName, "DeleteRouteCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "DeleteRouteCommand has been sent to the queue" });
        }
    }
}
