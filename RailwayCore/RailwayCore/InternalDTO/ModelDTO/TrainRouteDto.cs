using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class TrainRouteDto
    {
        public string Id { get; set; } = null!;
        public bool Is_Branded { get; set; } = false;
        public TrainQualityClass? Quality_Class { get; set; }
        public string? Branded_Name { get; set; }
        public SpeedType? Speed_Type { get; set; }
        public FrequencyType? Frequency_Type { get; set; }
        public AssignementType? Assignement_Type { get; set; }
        public string Railway_Branch_Title { get; set; } = null!;
    }
}
