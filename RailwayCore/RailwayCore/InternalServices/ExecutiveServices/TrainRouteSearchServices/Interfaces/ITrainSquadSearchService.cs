using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices.Interfaces
{
    public interface ITrainSquadSearchService
    {
        Task<List<PassengerCarriageOnTrainRouteOnDate>> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids);
        Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_on_date_id);
        Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_id, DateOnly departure_date);
    }
}