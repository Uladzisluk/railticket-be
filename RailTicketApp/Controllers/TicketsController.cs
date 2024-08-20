using Microsoft.AspNetCore.Mvc;
using RailTicketApp.Commands;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly CreateTicketCommandHandler _createCommandHandler;
        private readonly DeleteTicketCommandHandler _deleteCommandHandler;

        public TicketsController(CreateTicketCommandHandler createCommandHandler, DeleteTicketCommandHandler deleteCommandHandler)
        {
            _createCommandHandler = createCommandHandler;
            _deleteCommandHandler = deleteCommandHandler;
        }

        [HttpPost]
        public IActionResult CreateTicket([FromBody] CreateTicketCommand command)
        {
            _createCommandHandler.Handle(command);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var command = new DeleteTicketCommand { TicketId = id };
            _deleteCommandHandler.Handle(command);
            return Ok();
        }
    }
}
