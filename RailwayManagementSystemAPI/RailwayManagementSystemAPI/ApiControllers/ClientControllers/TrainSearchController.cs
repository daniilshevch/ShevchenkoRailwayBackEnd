using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayManagementSystemAPI.API_DTO;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.InternalServices.SystemServices;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [Route("ClientAPI/[controller]")]
    public class TrainSearchController : ControllerBase
    {
        private readonly TrainRouteWithBookingsSearchService train_route_with_booking_search_service;
        public TrainSearchController(TrainRouteWithBookingsSearchService train_route_with_booking_search_service)
        {
            this.train_route_with_booking_search_service = train_route_with_booking_search_service;
        }
        [HttpGet("Search-Train-Routes-Between-Stations-With-Bookings/{starting_station}/{ending_station}")]
        public async Task<ActionResult<List<ExternalTrainRouteWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfo(string starting_station, string ending_station, DateOnly departure_date)
        {
            QueryResult<List<ExternalTrainRouteWithBookingsInfoDto>>? train_routes_with_bookings_info_result =
                await train_route_with_booking_search_service.SearchTrainRoutesBetweenStationsWithBookingsInfo(starting_station, ending_station, departure_date, admin_mode: true);
            if (train_routes_with_bookings_info_result is FailQuery<List<ExternalTrainRouteWithBookingsInfoDto>>)
            {
                return NotFound(train_routes_with_bookings_info_result.Error!.Message);
            }
            return Ok(train_routes_with_bookings_info_result.Value);
        }
        [HttpPost("Get-Train-Schedule")]
        public ActionResult<List<ExternalSingleTrainStopDto>> GetScheduleForSpecificTrainRouteOnDate(ExternalTrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_with_booking_search_service.GetScheduleForSpecificTrainRouteOnDate(train_route_on_date);
        }
        [HttpPost("Get-Train-Schedule2")]
        public ActionResult<List<ExternalSingleTrainStopDto>> GetScheduleForSpecificTrainRouteOnDate(List<ExternalTrainRouteWithBookingsInfoDto> train_routes_on_date_statistics_list, string train_route_on_date_id)
        {
            return train_route_with_booking_search_service.GetScheduleForSpecificTrainRouteOnDateFromGeneralList(train_routes_on_date_statistics_list, train_route_on_date_id);
        }

        [HttpPost("Get-All-Carriages-Info")]
        public ActionResult<List<ExternalSinglePassengerCarriageBookingsInfoDto>> GetBookingsInfoForAllCarriagesForSpecificTrainRouteOnDate
            (ExternalTrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_with_booking_search_service.GetBookingsInfoForAllPassengerCarriagesForSpecificTrainRouteOnDate(train_route_on_date);
        }
        [HttpPost("Get-Carriages-Of-Type-Info")]
        public ActionResult<List<ExternalSinglePassengerCarriageBookingsInfoDto>> GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate
            (ExternalTrainRouteWithBookingsInfoDto train_route_on_date, PassengerCarriageType? carriage_type = null, PassengerCarriageQualityClass? quality_class = null)
        {
            return train_route_with_booking_search_service.GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate(train_route_on_date, carriage_type, quality_class);
        }
    }
}
