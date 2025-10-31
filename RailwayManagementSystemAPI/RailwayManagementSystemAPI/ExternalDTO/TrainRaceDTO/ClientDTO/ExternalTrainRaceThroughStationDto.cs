using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;
namespace RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO
{
    public class ExternalTrainRaceThroughStationDto
    {
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public string Train_Route_Id { get; set; } = null!;
        public string Full_Route_Starting_Station_Title { get; set; } = null!;
        public string Full_Route_Ending_Station_Title{ get; set; } = null!;
        public DateTime? Arrival_Time_To_Current_Stop { get; set; }
        public DateTime? Departure_Time_From_Current_Stop { get; set; }
        public double? Km_Point_Of_Current_Station { get; set; }
        public List<ExternalSingleTrainStopDto> Full_Route_Stops_List { get; set; } = new List<ExternalSingleTrainStopDto>();
    }
}
