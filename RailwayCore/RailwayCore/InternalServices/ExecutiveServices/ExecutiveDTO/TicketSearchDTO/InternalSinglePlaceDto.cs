using System.Text.Json.Serialization;

/// <summary>
/// Даний трансферний об'єкт містить інформацію про одне конкретне місце в одному конкретному вагоні в одному конкретному рейсі
/// (додатково може містити докладну інформацію про пасажира та характеристики його поїздки)
/// </summary>
[Checked("18.04.2025")]
public class InternalSinglePlaceDto //Представлення одного окремого місця в конкретному вагоні(номер місця, заброньованість місця, 
{                                   //інформація про пасажира, якщо місце заброньоване)
    [JsonPropertyName("place_in_carriage")]
    public int Place_In_Carriage { get; set; } //Номер місця в вагоні
    [JsonPropertyName("is_free")]
    public bool Is_Free { get; set; } //Чи місце заброньоване
    [JsonPropertyName("passenger_trip_info")]
    public List<ExternalPassengerTripInfoDto>? Passenger_Trip_Info { get; set; } //Інформація про поїздки пасажирів, які забронювали місце(не обов'язковий запис)
}