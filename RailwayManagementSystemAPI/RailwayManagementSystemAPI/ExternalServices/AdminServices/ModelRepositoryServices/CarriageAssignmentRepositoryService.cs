using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
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
        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> UpdatePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id,
            string train_route_on_date_id, ExternalCarriageAssignmentUpdateDto input)
        {
            PassengerCarriageOnTrainRouteOnDateUpdateDto update_dto = new PassengerCarriageOnTrainRouteOnDateUpdateDto()
            {
                Passenger_Carriage_Id = passenger_carriage_id,
                Train_Route_On_Date_Id = train_route_on_date_id,
                Position_In_Squad = input.Position_In_Squad,
                Factual_Air_Conditioning = input.Factual_Air_Conditioning,
                Factual_Is_Inclusive = input.Factual_Is_Inclusive,
                Factual_Shower_Availability = input.Factual_Shower_Availability,
                Food_Availability = input.Food_Availability,
                Is_For_Children = input.Is_For_Children,
                Is_For_Woman = input.Is_For_Woman
            };
            return await carriage_assignment_repository.UpdatePassengerCarriageOnTrainRouteOnDate(update_dto);
        }
    }
}
