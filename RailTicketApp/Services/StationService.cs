using RailTicketApp.Data;
using RailTicketApp.Models.Dto;
using RailTicketApp.Models;

namespace RailTicketApp.Services
{
    public class StationService
    {
        private readonly DbContextClass _dbContext;

        public StationService(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public List<StationDto> GetStations()
        {
            List<Station> stations = _dbContext.Stations.ToList();
            List<StationDto> stationDtos = new List<StationDto>();
            foreach (Station st in stations)
            {
                stationDtos.Add(new StationDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    City = st.City,
                    Country = st.Country
                });
            }
            return stationDtos;
        }
    }
}
