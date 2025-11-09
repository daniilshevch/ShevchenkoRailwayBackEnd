using System.ComponentModel.DataAnnotations;
using RailwayCore.Models.ModelEnums.UserEnums;

namespace RailwayCore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        public string Surname { get; set; } = null!;
        [MaxLength(30)]
        public string User_Name { get; set; } = null!;
        [MaxLength(30)]
        public string Name { get; set; } = null!;
        public Sex? Sex { get; set; }
        public Exemption? Exemption { get; set; }
        public double? Discount { get; set; }
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        [Phone]
        public string? Phone_Number { get; set; }
        public List<TicketBooking> Ticket_Bookings { get; set; } = new List<TicketBooking>();
        public UserProfile User_Profile { get; set; } = null!;
        public Role Role { get; set; } = Role.General_User;
    }
}
