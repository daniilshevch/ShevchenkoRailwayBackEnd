using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class StationDto
    {
        public int Id { get; set; }
        public string? Register_Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Location { get; set; }
        public StationType Type_Of { get; set; } = StationType.Mixed;
        public string? Locomotive_Depot { get; set; }
        public string? Carriage_Depot { get; set; }
        public string Railway_Branch_Title { get; set; } = null!;
        public Region? Region { get; set; }

        public static explicit operator StationDto(Station input)
        {
            return new StationDto()
            {
                Id = input.Id,
                Register_Id = input.Register_Id,
                Title = input.Title,
                Location = input.Location,
                Type_Of = input.Type_Of,
                Locomotive_Depot = input.Locomotive_Depot,
                Carriage_Depot = input.Carriage_Depot,
                Railway_Branch_Title = input.Railway_Branch.Title,
                Region = input.Region
            };
        }
    }
            
}
