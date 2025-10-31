using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO
{
    /// <summary>
    /// Представдяє собою групу вагонів одного типу в рейсі певного поїзда. Містить в собі словник груп, в кожній з яких містяться
    /// список вагони одного типу та класу якості
    /// </summary>
    [ClientDto]
    public class ExternalCarriageTypeGroupDto
    {
        [JsonPropertyName("free_places")]
        public int Free_Places { get; set; }
        [JsonPropertyName("total_places")]
        public int Total_Places { get; set; }
        [JsonPropertyName("min_price")]
        public double Min_Price { get; set; }
        [JsonPropertyName("max_price")]
        public double Max_Price { get; set; }
        [JsonPropertyName("carriage_quality_class_dictionary")]
        public Dictionary<string, ExternalCarriageTypeAndQualityGroupDto> Carriage_Quality_Class_Dictionary { get; set; } =
            new Dictionary<string, ExternalCarriageTypeAndQualityGroupDto>();
    }
}
