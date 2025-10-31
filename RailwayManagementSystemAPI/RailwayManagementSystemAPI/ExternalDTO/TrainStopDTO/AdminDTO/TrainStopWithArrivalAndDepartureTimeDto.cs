using RailwayCore.Models;

namespace RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.AdminDTO
{
    public class TrainStopWithArrivalAndDepartureTimeDto
    {
        public string Station_Title { get; set; } = null!;
        public DateTime? Arrival_Time { get; set; }
        public DateTime? Departure_Time { get; set; }
        public string Stop_Type { get; set; } = "Boarding";
        public double? Distance_From_Starting_Station { get; set; }
    }
}
