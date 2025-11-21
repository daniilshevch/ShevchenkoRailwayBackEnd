namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces
{
    public interface ITicketAvailabilityCheckService
    {
        Task<QueryResult<bool>> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title, string desired_ending_station_title, string passenger_carriage_id, int place_in_carrriage);
        Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title);
        Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics(List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title);
    }
}