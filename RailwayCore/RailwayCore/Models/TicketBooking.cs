using System.ComponentModel.DataAnnotations;

public enum TicketStatus
{
    Booking_In_Progress,
    Booked_And_Active,
    Booked_And_Used,
    Archieved,
    Returned
}
namespace RailwayCore.Models
{
    public class TicketBooking //Бронювання квитку
    {
        [Key]
        public int Id { get; set; } //Ідентифікатор квитка
        public Guid? Full_Ticket_Id { get; set; }
        public User User { get; set; } = null!; //Акаунт користувача в додатку, який придбав квиток
        public int User_Id { get; set; } //вище
        public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!; //Рейс поїзда, на який придбано квиток
        public string Train_Route_On_Date_Id { get; set; } = null!; //вище
        public PassengerCarriage Passenger_Carriage { get; set; } = null!; //Пасажирський вагон, в який куплено квиток(фізичний вагон через ідентифікатор)
        public string Passenger_Carriage_Id { get; set; } = null!; //вище
        public int? Passenger_Carriage_Position_In_Squad { get; set; } //Позиція, на якій вагон курсує в складі (**надлишковий/дискусійний)
        public Station Starting_Station { get; set; } = null!; //Стартова станція поїздки у квитку

        public int Starting_Station_Id { get; set; } //вище
        public Station Ending_Station { get; set; } = null!; //Кінцева станція поїздки у квитку
        public int Ending_Station_Id { get; set; } //вище
        public int Place_In_Carriage { get; set; } //Місце в вагоні
        [MaxLength(30)]
        public string Passenger_Name { get; set; } = null!; //Фактичне ім'я пасажира(може не збігатись з ім'ям в акаунті)
        [MaxLength(30)]
        public string Passenger_Surname { get; set; } = null!; //Фактичне прізвище пасажира(може не збігатись з прізвищем в акаунті)
        public DateTime Booking_Time { get; set; } //Час бронювання
        public DateTime? Booking_Expiration_Time { get; set; } // Час закінчення періоду бронювання(для квитків в статусі Booking_In_Progress
        public TicketStatus Ticket_Status { get; set; } = TicketStatus.Booked_And_Active; //Статус бронювання
        public string? Additional_Services { get; set; } //Стрічка для зберігання додаткових послуг, включених в квиток(постіль, напої, їжа і так далі)

    }
}
