namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.CarriageAssistantServices.Interfaces
{
    public interface ICarriageAssistantTicketAggregationService
    {
        Task<QueryResult<AdminFullTicketStatisticsInfoForPassengerCarriageDto>> GetGroupedTicketBookingsByStationsForTrainRace(string train_route_on_date_id, int carriage_position_in_squad, string? trip_starting_station_title, string? trip_ending_station_title, bool group_by_ending_station_first = false);
    }
}