using RailwayCore.Models;

[Checked("18.04.2025")]
public class InternalTrainRaceBetweenStationsDto  //Внутрішній трансфер(використовується в подальшому на сторінці пошуку поїздів між станціями)
{
    public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!;  //рейс поїзда
    public DateTime Departure_Time_From_Desired_Starting_Station { get; set; } //Час відправлення зі стартової станції поїздки(не всього рейсу)
    public TrainRouteOnDateOnStation Desired_Starting_Station { get; set; } = null!; //Початкова зупинка поїздки
    public DateTime Arrival_Time_For_Desired_Ending_Station { get; set; } //Час прибуття на кінцеву станцію поїздки
    public TrainRouteOnDateOnStation Desired_Ending_Station { get; set; } = null!; //Кінцева зупинка поїздки
    public double? Km_Point_Of_Desired_Starting_Station { get; set; } // Відстань від початкової станції всього рейсу до початкової станції поїздки
    public double? Km_Point_Of_Desired_Ending_Station { get; set; } // Відстань від початкової станції всього рейсу до кінцевої станції поїздки
    public TrainRouteOnDateOnStation Full_Route_Starting_Stop { get; set; } = null!; //Початкова станція всього рейсу
    public TrainRouteOnDateOnStation Full_Route_Ending_Stop { get; set; } = null!; //Кінцева станція всього рейсу
    public List<TrainRouteOnDateOnStation> Full_Route_Stops_List { get; set; } = new List<TrainRouteOnDateOnStation>(); //Весь список зупинок поїзда

}