using RailwayCore.Models;

/// <summary>
/// Внутрішній трансферний об'єкт, який передає інформацію про рейс поїзда між станціями поїздки(має інформацію про рейс загалом
/// та інформацію про рейс в контексті поїздки між двома конкретними станціями, через які цей рейс проходить.
/// </summary>
[Checked("18.04.2025")]
public class InternalTrainRaceBetweenStationsDto  
{
    public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!;  //рейс поїзда
    public DateTime Departure_Time_From_Desired_Starting_Station { get; set; } //Час відправлення зі стартової станції поїздки(не всього рейсу)
    public TrainRouteOnDateOnStation Desired_Starting_Station { get; set; } = null!; //Початкова зупинка поїздки(не рейсу)
    public DateTime Arrival_Time_For_Desired_Ending_Station { get; set; } //Час прибуття на кінцеву станцію поїздки(не рейсу)
    public TrainRouteOnDateOnStation Desired_Ending_Station { get; set; } = null!; //Кінцева зупинка поїздки(не рейсу0
    public double? Km_Point_Of_Desired_Starting_Station { get; set; } // Відстань від початкової станції всього рейсу до початкової станції поїздки
    public double? Km_Point_Of_Desired_Ending_Station { get; set; } // Відстань від початкової станції всього рейсу до кінцевої станції поїздки
    public TrainRouteOnDateOnStation Full_Route_Starting_Stop { get; set; } = null!; //Початкова станція всього рейсу(маршруту поїзда, не поїздки)
    public TrainRouteOnDateOnStation Full_Route_Ending_Stop { get; set; } = null!; //Кінцева станція всього рейсу(маршруту поїзда)
    public List<TrainRouteOnDateOnStation> Full_Route_Stops_List { get; set; } = new List<TrainRouteOnDateOnStation>(); //Весь список зупинок поїзда

}