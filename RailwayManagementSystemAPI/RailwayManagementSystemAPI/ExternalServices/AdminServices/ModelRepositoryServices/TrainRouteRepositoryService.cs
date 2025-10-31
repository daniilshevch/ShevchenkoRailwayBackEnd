using Microsoft.EntityFrameworkCore.Storage.Json;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TrainRouteDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class TrainRouteRepositoryService
    {
        private readonly TrainRouteRepository train_route_repository;
        public TrainRouteRepositoryService(TrainRouteRepository train_route_repository)
        {
            this.train_route_repository = train_route_repository;
        }
        public async Task<QueryResult<ExternalTrainRouteDto>> CreateTrainRoute(string id, ExternalTrainRouteCreateAndUpdateDto input)
        {
            TrainRouteDto train_route_dto = (TrainRouteDto)input;
            train_route_dto.Id = id;
            QueryResult<TrainRoute> train_creation_result =  await train_route_repository.CreateTrainRoute(train_route_dto);
            if(train_creation_result.Fail)
            {
                return new FailQuery<ExternalTrainRouteDto>(train_creation_result.Error);
            }
            return new SuccessQuery<ExternalTrainRouteDto>((ExternalTrainRouteDto)train_creation_result.Value);

        }
        public async Task<ExternalTrainRouteDto?> GetTrainRouteById(string id)
        {
            TrainRoute? train_route_get_result = await train_route_repository.GetTrainRouteById(id);
            if(train_route_get_result == null)
            {
                return null;
            }
            return (ExternalTrainRouteDto)train_route_get_result;
        }
        public async Task<List<ExternalTrainRouteDto>> GetTrainRoutes()
        {
            List<TrainRoute> train_routes_list = await train_route_repository.GetTrainRoutes();
            return train_routes_list.Select(train_route => (ExternalTrainRouteDto)train_route).ToList();
        }
        public async Task<QueryResult<ExternalTrainRouteDto>> UpdateTrainRoute(string id, ExternalTrainRouteCreateAndUpdateDto input)
        {
            TrainRouteDto train_route_dto = (TrainRouteDto)input;
            train_route_dto.Id = id;
            QueryResult<TrainRoute> train_route_update_result = await train_route_repository.UpdateTrainRoute(train_route_dto);
            if(train_route_update_result.Fail)
            {
                return new FailQuery<ExternalTrainRouteDto>(train_route_update_result.Error);
            }
            return new SuccessQuery<ExternalTrainRouteDto>((ExternalTrainRouteDto)train_route_update_result.Value);
        }
        public async Task<bool> DeleteTrainRouteById(string id)
        {
            return await train_route_repository.DeleteTrainRouteById(id);
        }
    }
}
