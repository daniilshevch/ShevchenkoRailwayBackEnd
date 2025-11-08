namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess
{
    /// <summary>
    /// Цей клас приймає метод контролера, який проводить фінальне оформлення квитка і його покупку. 
    /// Цей клас містить в собі проміжну інформацію про квиток, яку повернув метод, який ініціалізовував
    /// первинну резервацію квитка, а також інформацію про пасажира і його поїздку. За допомогою цієї інформації проводить фінальне
    /// оформлення та покупку квитка.
    /// </summary>
    public class ExternalInputCompletedTicketBookingWithPassengerInfoDto
    {
        public ExternalOutputMediatorTicketBookingDto ticket_booking_dto { get; set; } = null!; 
        public ExternalInputPassengerInfoForCompletedTicketBookingDto user_info_dto { get; set; } = null!;
    }
}
