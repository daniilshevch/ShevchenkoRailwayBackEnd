using RailwayCore.Models;
public class InternalTrainRaceThroughStationDto
{
    public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!;
    public TrainRouteOnDateOnStation Full_Route_Starting_Stop { get; set; } = null!;
    public TrainRouteOnDateOnStation Full_Route_Ending_Stop { get; set; } = null!;
    public TrainRouteOnDateOnStation Current_Stop { get; set; } = null!;
    public DateTime? Arrival_Time_To_Current_Stop { get; set; }
    public DateTime? Departure_Time_From_Current_Stop { get; set; }
    public double? Km_Point_Of_Current_Station { get; set; }
    public List<TrainRouteOnDateOnStation> Full_Route_Stops_List { get; set; } = new List<TrainRouteOnDateOnStation>();
}
