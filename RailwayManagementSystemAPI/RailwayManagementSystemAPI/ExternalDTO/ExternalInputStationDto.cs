using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalInputStationDto
    {
        public string? Register_Id { get; set; } 
        [MaxLength(30)]
        public string Title { get; set; } = null!; 
        [MaxLength(30)]
        public string? Ukrainian_Title { get; set; }
        [MaxLength(30)]
        public string? Location { get; set; } 
        [MaxLength(20)]
        public StationType Type_Of { get; set; } = StationType.Mixed; 
        public Region Region { get; set; } 
        [MaxLength(20)]
        public string? Locomotive_Depot { get; set; } 
        [MaxLength(20)]
        public string? Carriage_Depot { get; set; } 
        public string Railway_Branch_Title { get; set; } = null!; 

        public static explicit operator StationDto(ExternalInputStationDto input)
        {
            return new StationDto()
            {
                Register_Id = input.Register_Id,
                Title = input.Title,
                Location = input.Location,
                Type_Of = input.Type_Of,
                Region = input.Region,
                Locomotive_Depot = input.Locomotive_Depot,
                Carriage_Depot = input.Carriage_Depot,
            };
        }
    }
}
