using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.PassengerCarriageEnums;

namespace RailwayManagementSystemAPI.ExternalDTO.PassengerCarriageDTO.AdminDTO
{
    public class ExternalPassengerCarriageCreateAndUpdateDto
    {
        public PassengerCarriageType Type_Of { get; set; }
        public int Capacity { get; set; }
        public int? Production_Year { get; set; }
        public PassengerCarriageManufacturer? Manufacturer { get; set; }
        public PassengerCarriageQualityClass? Quality_Class { get; set; }
        public bool Renewal_Fact { get; set; }
        public int? Renewal_Year { get; set; }
        public PassengerCarriageManufacturer? Renewal_Performer { get; set; }
        public string? Renewal_Info { get; set; }
        public bool Air_Conditioning { get; set; } = false;
        public bool Is_Inclusive { get; set; } = false;
        public bool Is_For_Train_Chief { get; set; } = false;
        public bool Shower_Availability { get; set; } = false;
        public bool In_Current_Use { get; set; } = true;
        public string? Appearence { get; set; }
        public string? Station_Depot_Title { get; set; }
        public static explicit operator PassengerCarriageDto(ExternalPassengerCarriageCreateAndUpdateDto input)
        {
            return new PassengerCarriageDto()
            {
                Type_Of = input.Type_Of,
                Capacity = input.Capacity,
                Production_Year = input.Production_Year,
                Manufacturer = input.Manufacturer,
                Quality_Class = input.Quality_Class,
                Renewal_Fact = input.Renewal_Fact,
                Renewal_Year = input.Renewal_Year,
                Renewal_Performer = input.Renewal_Performer,
                Renewal_Info = input.Renewal_Info,
                Air_Conditioning = input.Air_Conditioning,
                Is_Inclusive = input.Is_Inclusive,
                Is_For_Train_Chief = input.Is_For_Train_Chief,
                Shower_Availability = input.Shower_Availability,
                In_Current_Use = input.In_Current_Use,
                Appearence = input.Appearence,
                Station_Depot_Title = input.Station_Depot_Title
            };
        }
    }
}
