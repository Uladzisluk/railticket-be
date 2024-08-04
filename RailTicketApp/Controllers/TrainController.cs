using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailTicketApp.Models;
using RailTicketApp.RabbitMQ;
using RailTicketApp.Services;

namespace RailTicketApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainController : ControllerBase
    {
        private readonly ITrainService trainService;
        private readonly IRabbitMQProducer _rabbitMQProducer;
        public TrainController(ITrainService _trainService, IRabbitMQProducer rabbitMQProducer)
        {
            trainService = _trainService;
            _rabbitMQProducer = rabbitMQProducer;
        }

        [HttpGet("trainlist")]
        public IEnumerable<Train> TrainList()
        {
            var trainList = trainService.GetTrainList();
            return trainList;
        }

        [HttpGet("gettrainbyid")]
        public Train GetTrainById(int id)
        {
            return trainService.GetTrainById(id);
        }

        [HttpPost("addtrain")]
        public Train AddTrain(Train train)
        {
            var trainData = trainService.AddTrain(train);
            //send the inserted product data to the queue and consumer will listening this data from queue
            _rabbitMQProducer.SendTrainMessage(trainData);
            return trainData;
        }

        [HttpPut("updatetrain")]
        public Train UpdateTrain(Train train)
        {
            return trainService.UpdateTrain(train);
        }

        [HttpDelete("deletetrain")]
        public bool DeleteTrain(int id)
        {
            return trainService.DeleteTrain(id);
        }
    }
}
