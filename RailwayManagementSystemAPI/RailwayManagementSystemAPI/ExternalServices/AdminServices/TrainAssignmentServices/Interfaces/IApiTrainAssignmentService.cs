using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models.ModelEnums.TrainStopEnums;
using RailwayManagementSystemAPI.ExternalDTO.TrainRouteDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.TrainAssignmentServices.Interfaces
{
    public interface IApiTrainAssignmentService
    {
        Task AssignTrainRouteOnDateWithSchedule(TrainRouteWithScheduleAssignmentDto input);
        Task AssignTrainRouteOnDateWithScheduleAndSquad(TrainRouteWithScheduleAndSquadAssignmentDto input);
        Task AssignTrainRouteOnDateWithSquad(TrainRouteWithSquadAssignmentDto input);
        Task ChangeTrainRouteOnDateSchedule(string train_route_id, DateOnly departure_date, List<TrainStopWithArrivalAndDepartureTimeDto> train_stops, bool deletion_option = true);
        Task ChangeTrainRouteOnDateSquad(string train_route_id, DateOnly departure_date, List<CarriageAssignementWithoutRouteDTO> carriage_assignments, bool deletion_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithSchedule(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithScheduleAndSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
        Task<QueryResult> CopyTrainRouteOnDateWithSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true);
        StopType GetStopTypeIntoEnum(string stop_type);
    }
}