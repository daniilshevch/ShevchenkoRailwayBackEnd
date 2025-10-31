using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO
{
    /// <summary>
    /// Цей клас представляє групу вагонів одного типу та класу якості. В кожній такій групі є список об'єктів 
    /// ExternalSinglePassengerCarriageBookingsInfoDto, кожен з яких представляє собою один вагон і інформацію про броні в ньому
    /// </summary>
    [ClientDto]
    public class ExternalCarriageTypeAndQualityGroupDto
    {
        [JsonPropertyName("free_places")]
        public int Free_Places { get; set; }
        [JsonPropertyName("total_places")]
        public int Total_Places { get; set; }
        [JsonPropertyName("min_price")]
        public double Min_Price { get; set; }
        [JsonPropertyName("max_price")]
        public double Max_Price { get; set; }
        [JsonPropertyName("carriage_statistics_list")]
        public List<ExternalSinglePassengerCarriageBookingsInfoDto> Carriage_Statistics_List { get; set; } = new List<ExternalSinglePassengerCarriageBookingsInfoDto>();

    }
}
