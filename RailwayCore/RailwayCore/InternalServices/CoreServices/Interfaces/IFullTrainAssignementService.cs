using RailwayCore.InternalDTO.ModelDTO;

namespace RailwayCore.InternalServices.CoreServices.Interfaces
{
    public interface IFullTrainAssignementService
    {
        Task ChangeTrainRouteOnDateSchedule(string train_route_id, DateOnly departure_date, List<TrainStopWithoutRouteDto> train_stops, bool deletion_option = true);
        Task ChangeTrainRouteOnDateSquad(string train_route_id, DateOnly departure_date, List<CarriageAssignementWithoutRouteDTO> carriage_assignments, bool deletion_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithSchedule(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithScheduleAndSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
        Task CreateTrainRouteOnDateWithSchedule(string train_route_id, DateOnly departure_date, List<TrainStopWithoutRouteDto> train_stops, bool creation_option = true);
        Task CreateTrainRouteOnDateWithScheduleAndSquad(string train_route_id, DateOnly departure_date, List<TrainStopWithoutRouteDto> train_stops, List<CarriageAssignementWithoutRouteDTO> carriage_assignements, bool creation_option = true);
        Task CreateTrainRouteOnDateWithSquad(string train_route_id, DateOnly departure_date, List<CarriageAssignementWithoutRouteDTO> carriage_assignements, bool creation_option = true);
    }
}