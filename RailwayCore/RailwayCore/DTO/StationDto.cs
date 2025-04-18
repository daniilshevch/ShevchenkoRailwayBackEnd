using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.DTO
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
        public Region Region { get; set; }

    }
}
