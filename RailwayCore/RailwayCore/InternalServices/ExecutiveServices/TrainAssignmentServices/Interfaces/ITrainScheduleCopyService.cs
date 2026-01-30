namespace RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Interfaces
{
    public interface ITrainScheduleCopyService
    {
        Task<QueryResult> CopyTrainRouteOnDateWithInvertedSchedule(string prototype_train_route_id, string new_inverted_train_route_id, DateOnly prototype_date, DateTime new_date_and_departure_time, bool creation_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithSchedule(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
    }
}