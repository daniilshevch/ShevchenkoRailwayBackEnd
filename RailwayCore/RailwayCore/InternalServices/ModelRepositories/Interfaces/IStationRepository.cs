using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface IStationRepository
    {
        Task<QueryResult<Station>> CreateStation(StationDto input);
        Task<bool> DeleteStation(int id);
        Task<Station?> GetStationById(int id);
        Task<Station?> GetStationByTitle(string title);
        Task<List<Station>> GetStations();
        Task<QueryResult<Station>> UpdateStation(StationDto input);
    }
}