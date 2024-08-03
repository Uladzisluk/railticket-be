using RailTicketApp.Models;
using RailTicketApp.Data;

namespace RailTicketApp.Services
{
    public class TrainService: ITrainService
    {
        private readonly DbContextClass _dbContext;
        public TrainService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Train> GetTrainList()
        {
            return _dbContext.Trains.ToList();
        }
        public Train GetTrainById(int id)
        {
            return _dbContext.Trains.Where(x => x.Id == id).FirstOrDefault();
        }
        public Train AddTrain(Train train)
        {
            var result = _dbContext.Trains.Add(train);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        public Train UpdateTrain(Train train)
        {
            var result = _dbContext.Trains.Update(train);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        public bool DeleteTrain(int id)
        {
            var filteredData = _dbContext.Trains.Where(x => x.Id == id).FirstOrDefault();
            var result = _dbContext.Remove(filteredData);
            _dbContext.SaveChanges();
            return result != null ? true : false;
        }
    }
}
