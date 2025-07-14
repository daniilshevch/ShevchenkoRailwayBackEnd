using RailwayCore.Models;

[Checked("18.04.2025")]
public class InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto //Інформація про бронювання місць в усіх вагонах одного поїзда
{
    public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!; //Інформація про рейс поїзда(маршрут поїзда в дату)

    //Список об'єктів-трансферів, де кожний об'єкт представляє інформацію про бронювання в одному вагоні
    public List<InternalCarriageAssignmentRepresentationDto> Carriage_Statistics_List { get; set; } = new List<InternalCarriageAssignmentRepresentationDto>();

}