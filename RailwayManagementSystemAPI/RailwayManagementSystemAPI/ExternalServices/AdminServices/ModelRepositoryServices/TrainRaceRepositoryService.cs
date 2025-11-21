using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.AdminDTO;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class TrainRaceRepositoryService
    {
        private readonly ITrainRouteOnDateRepository train_route_on_date_repository;
        public TrainRaceRepositoryService(ITrainRouteOnDateRepository train_route_on_date_repository)
        {
            this.train_route_on_date_repository = train_route_on_date_repository;
        }
        public async Task<ExternalSimpleTrainRaceDto?> CreateTrainRouteOnDate(TrainRouteOnDateDto input)
        {
            TrainRouteOnDate? train_race = await train_route_on_date_repository.CreateTrainRouteOnDate(input);
            if(train_race is null)
            {
                return null;
            }
            return (ExternalSimpleTrainRaceDto)train_race;
        }
        public async Task<List<ExternalSimpleTrainRaceDto>> GetTrainRoutesOnDateForTrainRoute(string train_route_id)
        {
            List<TrainRouteOnDate> train_races = await train_route_on_date_repository.GetTrainRoutesOnDateForTrainRoute(train_route_id);
            return train_races.Select(train_race => (ExternalSimpleTrainRaceDto)train_race).ToList();
        }
        public async Task<ExternalSimpleTrainRaceDto?> ChangeTrainRaceCoefficientForTrainRouteOnDate(string train_route_on_date_id, double train_race_coefficient)
        {
            TrainRouteOnDate? train_race = await train_route_on_date_repository.ChangeTrainRaceCoefficientForTrainRouteOnDate(train_route_on_date_id, train_race_coefficient);
            if(train_race is null)
            {
                return null;
            }
            return (ExternalSimpleTrainRaceDto)train_race;
        }
        public async Task<bool> DeleteTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_route_on_date_repository.DeleteTrainRouteOnDate(train_route_on_date_id);
        }
    }
}
