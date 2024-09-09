using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailTicketApp.Commands.Tickets;
using RailTicketApp.Models.Dto;
using RailTicketApp.RabbitMq;
using RailTicketApp.Services;

namespace RailTicketApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly RabbitMqSender _rabbitMqSender;
        private readonly RabbitMqSettings _settings;
        private readonly TicketService _ticketService;

        public TicketsController(IOptions<RabbitMqSettings> settings, RabbitMqSender rabbitMqSender, TicketService ticketService)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _rabbitMqSender = rabbitMqSender;
            _ticketService = ticketService;
        }

        [HttpGet]
        public IActionResult GetTickets()
        {
            var tickets = _ticketService.GetTickets();

            if (tickets == null || !tickets.Any())
            {
                var emptyResponse = ResponseFactory.Ok("", 200, "No tickets found");
                _rabbitMqSender.Dispose();
                return Ok(emptyResponse);
            }

            _rabbitMqSender.Dispose();
            return Ok(ResponseFactory.Ok(tickets, 200, "Tickets retrieved successfully"));
        }

        [HttpPost]
        public IActionResult CreateTicket([FromBody] CreateTicketCommand command, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            _rabbitMqSender.SendMessage(command, _settings.TicketQueueName, "CreateTicketCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "CreateTicketCommand has been sent to the queue" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id, [FromHeader(Name = "CorrelationId")] string correlationId)
        {
            var command = new DeleteTicketCommand { TicketId = id };
            _rabbitMqSender.SendMessage(command, _settings.TicketQueueName, "DeleteTicketCommand", correlationId);
            _rabbitMqSender.Dispose();
            return Ok(new { Message = "DeleteTicketCommand has been sent to the queue" });
        }
    }
}
