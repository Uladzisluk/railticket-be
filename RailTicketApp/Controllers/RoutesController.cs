using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Routes;
using RailTicketApp.Models;
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
        private readonly TrainService _trainService;
        private readonly BookingService _bookingService;
        private readonly RabbitMqSettings _settings;

        public RoutesController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender, RouteService routeService, TrainService trainService, BookingService bookingService)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
            _routeService = routeService;
            _trainService = trainService;
            _bookingService = bookingService;
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

        [HttpGet("${routeId}/Train")]
        public IActionResult GetRouteTrain(int routeId)
        {
            try
            {
                var route = _routeService.GetRoute(routeId);
                var trainDto = _trainService.GetTrainDto(route.TrainId);
                _rabbitMqSender?.Dispose();
                return Ok(ResponseFactory.Ok(trainDto, 200, "Train retrieved successfully"));
            }
            catch(NullReferenceException ex)
            {
                _rabbitMqSender?.Dispose();
                return BadRequest(ResponseFactory.Error("", 400, ex.GetType().Name, ex.Message));
            }

        }

        [HttpGet("${routeId}/Bookings")]
        public IActionResult GetRouteBookings(int routeId)
        {
            var bookings = _bookingService.GetBookingDtosByRouteId(routeId);
            if(bookings == null || !bookings.Any())
            {
                var emptyResponse = ResponseFactory.Ok("", 200, "No bookings found");
                _rabbitMqSender.Dispose();
                return Ok(emptyResponse);
            }
            _rabbitMqSender.Dispose();
            return Ok(ResponseFactory.Ok(bookings, 200, "Bookings retrieved successfully"));
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
