using RailwayCore.InternalServices.ModelServices;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class StationRepositoryService
    {
        private readonly StationRepository station_repository;
        public StationRepositoryService(StationRepository station_repository)
        {
            this.station_repository = station_repository;
        }
    }
}
