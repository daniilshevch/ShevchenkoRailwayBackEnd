using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface IPassengerCarriageRepository
    {
        Task<QueryResult<PassengerCarriage>> CreatePassengerCarriage(PassengerCarriageDto input);
        Task<bool> DeletePassengerCarriage(string passenger_carriage_id);
        Task<List<PassengerCarriage>> GetAllPassengerCarriages();
        Task<PassengerCarriage?> GetPassengerCarriageById(string id);
        Task<QueryResult<PassengerCarriage>> UpdatePassengerCarriage(PassengerCarriageDto input);
    }
}