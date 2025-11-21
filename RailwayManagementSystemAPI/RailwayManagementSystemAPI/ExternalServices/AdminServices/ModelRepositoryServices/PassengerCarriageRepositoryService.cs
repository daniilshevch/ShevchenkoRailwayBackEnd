using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.PassengerCarriageDTO.AdminDTO;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class PassengerCarriageRepositoryService
    {
        private readonly IPassengerCarriageRepository passenger_carriage_repository;
        public PassengerCarriageRepositoryService(IPassengerCarriageRepository passenger_carriage_repository)
        {
            this.passenger_carriage_repository = passenger_carriage_repository;
        }
        public async Task<QueryResult<PassengerCarriageDto>> CreatePassengerCarriage(string passenger_carriage_id, ExternalPassengerCarriageCreateAndUpdateDto input)
        {
            PassengerCarriageDto passenger_carriage_dto = (PassengerCarriageDto)input;
            passenger_carriage_dto.Id = passenger_carriage_id;

            QueryResult<PassengerCarriage> passenger_carriage_creation_result = await passenger_carriage_repository.CreatePassengerCarriage(passenger_carriage_dto);
            if(passenger_carriage_creation_result.Fail)
            {
                return new FailQuery<PassengerCarriageDto>(passenger_carriage_creation_result.Error);
            }
            return new SuccessQuery<PassengerCarriageDto>((PassengerCarriageDto)passenger_carriage_creation_result.Value);
        }
        public async Task<PassengerCarriageDto?> GetPassengerCarriageById(string id)
        {
            PassengerCarriage? passenger_carriage = await passenger_carriage_repository.GetPassengerCarriageById(id);   
            if(passenger_carriage is null)
            {
                return null;
            }
            return (PassengerCarriageDto)passenger_carriage;
        }
        public async Task<List<PassengerCarriageDto>> GetAllPassengerCarriages()
        {
            List<PassengerCarriage> passenger_carriages = await passenger_carriage_repository.GetAllPassengerCarriages();
            return passenger_carriages.Select(passenger_carriage => (PassengerCarriageDto)passenger_carriage).ToList();
        }
        public async Task<QueryResult<PassengerCarriageDto>> UpdatePassengerCarriage(string passenger_carriage_id, ExternalPassengerCarriageCreateAndUpdateDto input)
        {
            PassengerCarriageDto passenger_carriage_dto = (PassengerCarriageDto)input;
            passenger_carriage_dto.Id = passenger_carriage_id;
            QueryResult<PassengerCarriage> passenger_carriage_update_result = await passenger_carriage_repository.UpdatePassengerCarriage(passenger_carriage_dto);
            if(passenger_carriage_update_result.Fail)
            {
                return new FailQuery<PassengerCarriageDto>(passenger_carriage_update_result.Error);
            }
            return new SuccessQuery<PassengerCarriageDto>((PassengerCarriageDto)passenger_carriage_update_result.Value);
        }
        public async Task<bool> DeletePassengerCarriage(string passenger_carriage_id)
        {
            return await passenger_carriage_repository.DeletePassengerCarriage(passenger_carriage_id);
        }
    }
}
