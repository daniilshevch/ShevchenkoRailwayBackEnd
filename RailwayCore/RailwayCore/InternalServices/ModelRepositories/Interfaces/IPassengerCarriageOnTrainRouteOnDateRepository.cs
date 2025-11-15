using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface IPassengerCarriageOnTrainRouteOnDateRepository
    {
        Task<PassengerCarriageOnTrainRouteOnDate?> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDate input);
        Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input);
        Task<bool> DeletePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id, string train_route_on_date_id);
        Task<QueryResult<List<PassengerCarriageOnTrainRouteOnDate>>> GetPassengerCarriagesForTrainRouteOnDate(string train_route_on_date_id);
        Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> UpdatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input);
    }
}