using RailTicketApp.Models;

namespace RailTicketApp.Services
{
    public interface ITrainService
    {
        public IEnumerable<Train> GetTrainList();
        public Train GetTrainById(int id);
        public Train AddTrain(Train train);
        public Train UpdateTrain(Train train);
        public bool DeleteTrain(int id);
    }
}
