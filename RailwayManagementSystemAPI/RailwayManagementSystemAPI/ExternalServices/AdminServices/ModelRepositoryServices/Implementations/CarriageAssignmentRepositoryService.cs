using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.AdminDTO;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations
{
    public class CarriageAssignmentRepositoryService : ICarriageAssignmentRepositoryService
    {
        private readonly IPassengerCarriageOnTrainRouteOnDateRepository carriage_assignment_repository;
        public CarriageAssignmentRepositoryService(IPassengerCarriageOnTrainRouteOnDateRepository carriage_assignment_repository)
        {
            this.carriage_assignment_repository = carriage_assignment_repository;
        }
        public async Task<QueryResult<ExternalCarriageAssignmentDto>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input)
        {
            QueryResult<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_creation_result =
                await carriage_assignment_repository.CreatePassengerCarriageOnTrainRouteOnDate(input);
            if (carriage_assignment_creation_result.Fail)
            {
                return new FailQuery<ExternalCarriageAssignmentDto>(carriage_assignment_creation_result.Error);
            }
            return new SuccessQuery<ExternalCarriageAssignmentDto>
                ((ExternalCarriageAssignmentDto)carriage_assignment_creation_result.Value);
        }

        public async Task<QueryResult<ExternalCarriageAssignmentDto>> UpdatePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id,
            string train_route_on_date_id, ExternalCarriageAssignmentUpdateDto input)
        {
            PassengerCarriageOnTrainRouteOnDateDto update_dto = new PassengerCarriageOnTrainRouteOnDateDto()
            {
                Passenger_Carriage_Id = passenger_carriage_id,
                Train_Route_On_Date_Id = train_route_on_date_id,
                Position_In_Squad = input.Position_In_Squad,
                Factual_Wi_Fi = input.Factual_Wi_Fi,
                Factual_Air_Conditioning = input.Factual_Air_Conditioning,
                Factual_Is_Inclusive = input.Factual_Is_Inclusive,
                Factual_Shower_Availability = input.Factual_Shower_Availability,
                Food_Availability = input.Food_Availability,
                Is_For_Children = input.Is_For_Children,
                Is_For_Woman = input.Is_For_Woman
            };
            QueryResult<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_update_result =
                await carriage_assignment_repository.UpdatePassengerCarriageOnTrainRouteOnDate(update_dto);
            if (carriage_assignment_update_result.Fail)
            {
                return new FailQuery<ExternalCarriageAssignmentDto>(carriage_assignment_update_result.Error);
            }
            return new SuccessQuery<ExternalCarriageAssignmentDto>
                ((ExternalCarriageAssignmentDto)carriage_assignment_update_result.Value);
        }
        public async Task<QueryResult<List<ExternalCarriageAssignmentDto>>> GetPassengerCarriagesForTrainRouteOnDate(string train_route_on_date_id)
        {
            QueryResult<List<PassengerCarriageOnTrainRouteOnDate>> carriage_assignment_get_result =
                await carriage_assignment_repository.GetPassengerCarriagesForTrainRouteOnDate(train_route_on_date_id);
            if (carriage_assignment_get_result.Fail)
            {
                return new FailQuery<List<ExternalCarriageAssignmentDto>>(carriage_assignment_get_result.Error);
            }
            List<ExternalCarriageAssignmentDto> carriage_assignments = carriage_assignment_get_result.Value.Select(single_carriage_assignment =>
            (ExternalCarriageAssignmentDto)single_carriage_assignment).ToList();
            return new SuccessQuery<List<ExternalCarriageAssignmentDto>>(carriage_assignments);

        }
        public async Task<bool> DeletePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id, string train_route_on_date_id)
        {
            return await carriage_assignment_repository.DeletePassengerCarriageOnTrainRouteOnDate(passenger_carriage_id, train_route_on_date_id);
        }
    }
}
