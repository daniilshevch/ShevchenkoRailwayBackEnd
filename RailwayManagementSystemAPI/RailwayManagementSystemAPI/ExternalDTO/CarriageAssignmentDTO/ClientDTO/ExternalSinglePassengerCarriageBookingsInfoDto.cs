using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO
{
    /// <summary>
    /// Представляє собою один вагон в складі рейса поїзда: докладну інформацію про нього, а також інформацію
    /// про бронювання місць в даному вагоні в даному рейсі
    /// </summary>
    [ClientDto]
    public class ExternalSinglePassengerCarriageBookingsInfoDto
    {
        [JsonPropertyName("carriage_position_in_squad")]
        public int Carriage_Position_In_Squad { get; set; } //Позиція вагона в складі поїзда
        [JsonPropertyName("carriage_type")]
        public string Carriage_Type { get; set; } = null!; //Тип вагона
        [JsonPropertyName("carriage_quality_class")]
        public string? Quality_Class { get; set; } //Підклас вагону
        [JsonPropertyName("ticket_price")]
        public int Ticket_Price { get; set; } //Ціна квитку в даному вагоні
        [JsonPropertyName("free_places")]
        public int Free_Places { get; set; } //Кількість вільних місць в вагоні
        [JsonPropertyName("total_places")]
        public int Total_Places { get; set; } //Загальна кількість місць в вагоні
        [JsonPropertyName("air_conditioning")]
        public bool Air_Conditioning { get; set; } //Наявність кондиціонування
        [JsonPropertyName("shower_availability")]
        public bool Shower_Availability { get; set; } //Наявність душу
        [JsonPropertyName("food_availability")]
        public bool Food_Availability { get; set; } //Доступність їжі в вагоні
        [JsonPropertyName("wifi_availability")]
        public bool WiFi_Availability { get; set; } //Наявність вай-фай
        [JsonPropertyName("is_inclusive")]
        public bool Is_Inclusive { get; set; } //Чи пристосований для перевезення пасажирів з відхиленнями
        [JsonPropertyName("places_availability_list")]
        public List<InternalSinglePlaceDto> Places_Availability { get; set; } = new List<InternalSinglePlaceDto>();
    }
}
