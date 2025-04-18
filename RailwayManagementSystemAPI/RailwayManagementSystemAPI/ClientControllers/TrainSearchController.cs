using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayManagementSystemAPI.API_DTO;
using RailwayManagementSystemAPI.ClientServices;
namespace RailwayManagementSystemAPI.Controllers
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
        [HttpGet("Search-Train-Routes-Between-Stations-With-Bookings")]
        public async Task<ActionResult<List<TrainRouteWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfo(string starting_station, string ending_station, DateOnly departure_date)
        {
            List<TrainRouteWithBookingsInfoDto>? train_routes_with_bookings_info =
                await train_route_with_booking_search_service.SearchTrainRoutesBetweenStationsWithBookingsInfo(starting_station, ending_station, departure_date);
            return Ok(train_routes_with_bookings_info);
        }
        [HttpPost("Get-Train-Schedule")]
        public ActionResult<List<SingleTrainStopDto>> GetScheduleForSpecificTrainRouteOnDate(TrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_with_booking_search_service.GetScheduleForSpecificTrainRouteOnDate(train_route_on_date);
        }
        [HttpPost("Get-All-Carriages-Info")]
        public ActionResult<List<SinglePassengerCarriageBookingsInfoDto>> GetBookingsInfoForAllCarriagesForSpecificTrainRouteOnDate
            (TrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_with_booking_search_service.GetBookingsInfoForAllPassengerCarriagesForSpecificTrainRouteOnDate(train_route_on_date);
        }
        [HttpPost("Get-Carriages-Of-Type-Info")]
        public ActionResult<List<SinglePassengerCarriageBookingsInfoDto>> GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate
            (TrainRouteWithBookingsInfoDto train_route_on_date, PassengerCarriageType? carriage_type = null, PassengerCarriageQualityClass? quality_class = null)
        {
            return train_route_with_booking_search_service.GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate(train_route_on_date, carriage_type, quality_class);
        }
    }
}
