using RailwayCore.Models.ModelEnums.PassengerCarriageEnums;
using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface ITrainRouteWithBookingsSearchService
    {
        List<ExternalSinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForAllPassengerCarriagesForSpecificTrainRouteOnDate(ExternalTrainRaceWithBookingsInfoDto train_route_on_date);
        List<ExternalSinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate(ExternalTrainRaceWithBookingsInfoDto train_route_on_date, PassengerCarriageType? carriage_type = null, PassengerCarriageQualityClass? quality_class = null);
        Task<QueryResult<ExternalTrainRaceWithBookingsInfoDto>> GetCompleteInfoWithBookingsForTrainRaceOnDateBetweenStations(string train_route_on_date_id, string starting_station_title, string ending_station_title, bool admin_mode = false, bool places_availability_info = true);
        List<ExternalSingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDate(ExternalTrainRaceWithBookingsInfoDto train_route_on_date);
        List<ExternalSingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDateFromGeneralList(List<ExternalTrainRaceWithBookingsInfoDto> train_routes_on_date_statistics_list, string train_route_id);
        Task<QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfo(string starting_station_title, string ending_station_title, DateOnly departure_date, bool admin_mode = false, bool places_availability_info = true);
        Task<QueryResult<List<ExternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStation(string station_title, DateTime time, TimeSpan? left_interval = null, TimeSpan? right_interval = null);
        QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>> _CreateListOfExternalTrainRaceWithBookingsInfoDto(List<InternalTrainRaceBetweenStationsDto> appropriate_train_routes_on_date, Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> ticket_bookings_info_for_appropriate_train_routes, string starting_station_title, string ending_station_title, bool include_places_availability_info = true);
        (ExternalTrainRaceWithBookingsInfoDto? fastest_train_race, ExternalTrainRaceWithBookingsInfoDto? cheapest_train_race) _DefineAvailableTrainRacesTopChart(List<ExternalTrainRaceWithBookingsInfoDto> total_train_routes_with_bookings_and_stations_info);
        Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> _GetAppropriateTrainRoutesBetweenStationsOnDate(string starting_station_title, string ending_station_title, DateOnly departure_date);
    }
}