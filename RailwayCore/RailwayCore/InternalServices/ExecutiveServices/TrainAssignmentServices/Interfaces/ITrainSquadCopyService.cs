namespace RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Interfaces
{
    public interface ITrainSquadCopyService
    {
        Task<QueryResult> CopyTrainRouteOnDateWithSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
    }
}