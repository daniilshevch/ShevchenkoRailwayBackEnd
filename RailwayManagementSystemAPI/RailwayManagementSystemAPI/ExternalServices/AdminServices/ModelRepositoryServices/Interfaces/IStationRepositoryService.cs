using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalDTO.StationDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface IStationRepositoryService
    {
        Task<QueryResult<StationDto>> CreateStation(int id, ExternalInputStationDto input);
        Task<bool> DeleteStation(int id);
        Task<StationDto?> GetStationById(int id);
        Task<StationDto?> GetStationByTitle(string title);
        Task<List<StationDto>> GetStations();
        Task<QueryResult<StationDto>> UpdateStation(int id, ExternalInputStationDto input);
    }
}