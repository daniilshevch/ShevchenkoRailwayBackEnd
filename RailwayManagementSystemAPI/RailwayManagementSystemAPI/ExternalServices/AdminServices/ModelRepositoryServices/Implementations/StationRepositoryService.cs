using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.StationDTO.AdminDTO;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations
{
    public class StationRepositoryService : IStationRepositoryService
    {
        private readonly IStationRepository station_repository;
        public StationRepositoryService(IStationRepository station_repository)
        {
            this.station_repository = station_repository;
        }
        public async Task<QueryResult<StationDto>> CreateStation(int id, ExternalInputStationDto input)
        {
            StationDto station_dto = (StationDto)input;
            station_dto.Id = id;
            QueryResult<Station> station_creation_result = await station_repository.CreateStation(station_dto);
            if (station_creation_result.Fail)
            {
                return new FailQuery<StationDto>(station_creation_result.Error);
            }
            return new SuccessQuery<StationDto>((StationDto)station_creation_result.Value);
        }
        public async Task<StationDto?> GetStationById(int id)
        {
            Station? station = await station_repository.GetStationById(id);
            if (station is null)
            {
                return null;
            }
            return (StationDto)station;
        }
        public async Task<StationDto?> GetStationByTitle(string title)
        {
            Station? station = await station_repository.GetStationByTitle(title);
            if (station is null)
            {
                return null;
            }
            return (StationDto)station;
        }
        public async Task<List<StationDto>> GetStations()
        {
            List<Station> stations = await station_repository.GetStations();
            return stations.Select(station => (StationDto)station).ToList();
        }
        public async Task<QueryResult<StationDto>> UpdateStation(int id, ExternalInputStationDto input)
        {
            StationDto station_dto = (StationDto)input;
            station_dto.Id = id;
            QueryResult<Station> station_update_result = await station_repository.UpdateStation(station_dto);
            if (station_update_result.Fail)
            {
                return new FailQuery<StationDto>(station_update_result.Error);
            }
            return new SuccessQuery<StationDto>((StationDto)station_update_result.Value);

        }
        public async Task<bool> DeleteStation(int id)
        {
            return await station_repository.DeleteStation(id);
        }

    }
}
