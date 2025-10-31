using RailwayCore.Models;

/// <summary>
/// Даний об'єкт трансфер містить інформацію про рейс поїзда, в плані його складу у вигляді вагонів, а також всіх заброньованих місць у ньому
/// (в контексті поїздки між певними станціями)
/// </summary>
[Checked("18.04.2025")]
public class InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto //Інформація про бронювання місць в усіх вагонах одного поїзда
{
    public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!; //Інформація про рейс поїзда(маршрут поїзда в дату)

    //Список об'єктів-трансферів, де кожний об'єкт представляє інформацію про бронювання в одному вагоні
    public List<InternalSinglePassengerCarriageAssignmentRepresentationDto> Carriage_Statistics_List { get; set; } = new List<InternalSinglePassengerCarriageAssignmentRepresentationDto>();

}