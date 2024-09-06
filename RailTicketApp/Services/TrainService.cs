using RailTicketApp.Data;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;

namespace RailTicketApp.Services
{
    public class TrainService
    {
        private readonly DbContextClass _dbContext;

        public TrainService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TrainDto> GetTrains()
        {
            List<Train> trains = _dbContext.Trains.ToList();
            List<TrainDto> trainDtos = new List<TrainDto>();
            foreach(Train tr in trains)
            {
                trainDtos.Add(new TrainDto
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Number = tr.Number
                });
            }
            return trainDtos;
        }
    }
}
