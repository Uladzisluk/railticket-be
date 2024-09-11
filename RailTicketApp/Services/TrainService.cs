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
                    Number = tr.Number,
                    TotalSeats = tr.TotalSeats
                });
            }
            return trainDtos;
        }

        public TrainDto GetTrainDto(int id)
        {
            Train train = _dbContext.Trains.Find(id);
            if (train == null)
            {
                throw new NullReferenceException("There is no train with id " + id + " in db");
            }
            return new TrainDto
            {
                Id = train.Id,
                Name = train.Name,
                Number = train.Number,
                TotalSeats = train.TotalSeats
            };
        }
    }
}
