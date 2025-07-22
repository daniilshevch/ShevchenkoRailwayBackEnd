using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;

namespace RailwayManagementSystemAPI.ExternalServices.ModelRepositoryServices
{
    public class CarriageAssignmentRepositoryService
    {
        private readonly PassengerCarriageOnTrainRouteOnDateRepository carriage_assignment_repository;
        public CarriageAssignmentRepositoryService(PassengerCarriageOnTrainRouteOnDateRepository carriage_assignment_repository)
        {
            this.carriage_assignment_repository = carriage_assignment_repository;
        }
        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input)
        {
            return await carriage_assignment_repository.CreatePassengerCarriageOnTrainRouteOnDate(input);
        }
    }
}
