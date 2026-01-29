using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalDTO.PassengerCarriageDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface IPassengerCarriageRepositoryService
    {
        Task<QueryResult<PassengerCarriageDto>> CopyPassengerCarriage(string new_passenger_carriaged_id, string prototype_passenger_carriage_id);
        Task<QueryResult<PassengerCarriageDto>> CreatePassengerCarriage(string passenger_carriage_id, ExternalPassengerCarriageCreateAndUpdateDto input);
        Task<bool> DeletePassengerCarriage(string passenger_carriage_id);
        Task<List<PassengerCarriageDto>> GetAllPassengerCarriages();
        Task<PassengerCarriageDto?> GetPassengerCarriageById(string id);
        Task<QueryResult<PassengerCarriageDto>> UpdatePassengerCarriage(string passenger_carriage_id, ExternalPassengerCarriageCreateAndUpdateDto input);
    }
}