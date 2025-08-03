using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using System.Reflection.Metadata.Ecma335;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class CarriageAssignmentRepositoryService
    {
        private readonly PassengerCarriageOnTrainRouteOnDateRepository carriage_assignment_repository;
        public CarriageAssignmentRepositoryService(PassengerCarriageOnTrainRouteOnDateRepository carriage_assignment_repository)
        {
            this.carriage_assignment_repository = carriage_assignment_repository;
        }
        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDateDto>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input)
        {
            QueryResult<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_creation_result =  
                await carriage_assignment_repository.CreatePassengerCarriageOnTrainRouteOnDate(input);
            if(carriage_assignment_creation_result.Fail)
            {
                return new FailQuery<PassengerCarriageOnTrainRouteOnDateDto>(carriage_assignment_creation_result.Error);
            }
            return new SuccessQuery<PassengerCarriageOnTrainRouteOnDateDto>
                ((PassengerCarriageOnTrainRouteOnDateDto)carriage_assignment_creation_result.Value);
        }

        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDateDto>> UpdatePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id,
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
                return new FailQuery<PassengerCarriageOnTrainRouteOnDateDto>(carriage_assignment_update_result.Error);
            }
            return new SuccessQuery<PassengerCarriageOnTrainRouteOnDateDto>
                ((PassengerCarriageOnTrainRouteOnDateDto)carriage_assignment_update_result.Value);
        }
        public async Task<QueryResult<List<PassengerCarriageOnTrainRouteOnDateDto>>> GetPassengerCarriagesForTrainRouteOnDate(string train_route_on_date_id)
        {
            QueryResult<List<PassengerCarriageOnTrainRouteOnDate>> carriage_assignment_get_result = 
                await carriage_assignment_repository.GetPassengerCarriagesForTrainRouteOnDate(train_route_on_date_id);
            if(carriage_assignment_get_result.Fail)
            {
                return new FailQuery<List<PassengerCarriageOnTrainRouteOnDateDto>>(carriage_assignment_get_result.Error);
            }
            List<PassengerCarriageOnTrainRouteOnDateDto> carriage_assignments = carriage_assignment_get_result.Value.Select(single_carriage_assignment =>
            (PassengerCarriageOnTrainRouteOnDateDto)single_carriage_assignment).ToList();
            return new SuccessQuery<List<PassengerCarriageOnTrainRouteOnDateDto>>(carriage_assignments);

        }
        public async Task<bool> DeletePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id, string train_route_on_date_id)
        {
            return await carriage_assignment_repository.DeletePassengerCarriageOnTrainRouteOnDate(passenger_carriage_id, train_route_on_date_id);
        }
    }
}
