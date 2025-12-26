using Microsoft.AspNetCore.Identity.UI.Services;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Implementations
{
    /// <summary>
    /// Даний сервіс проводить розсилку квитків на пошту користувача після покупки через сервіси SendGrid
    /// </summary>
    public class SendGridTicketSender: IEmailTicketSender
    {
        private readonly string api_key;
        private readonly string from_email;
        public SendGridTicketSender(IConfiguration configuration)
        {
            api_key = configuration["SendGrid:ApiKey"]!;
            from_email = configuration["SendGrid:Mail"]!;
        }
        /// <summary>
        /// Відправляє квиток на пошту користувача після покупки
        /// </summary>
        /// <param name="user_email"></param>
        /// <param name="ticket_booking_info"></param>
        /// <returns></returns>
        public async Task<QueryResult> SendTicketToEmailAsync(string user_email, TicketBooking ticket_booking_info)
        {
            SendGridClient client = new SendGridClient(api_key);
            EmailAddress from = new EmailAddress(from_email, "Shevchenko Railway");
            EmailAddress to = new EmailAddress(user_email);
            string subject = "Your ticket";

            var htmlContent = $"<p>Привіт, {ticket_booking_info.Passenger_Name}!</p><p>Твій квиток: <strong>{ticket_booking_info.Starting_Station.Title}</strong></p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "test", htmlContent);
            Response response = await client.SendEmailAsync(msg);
            Console.WriteLine(response.IsSuccessStatusCode);
            return new SuccessQuery(new SuccessMessage($""));
        }
        public async Task<QueryResult> SendMultipleTicketsToEmail(string user_email, List<ExternalOutputCompletedTicketBookingDto> ticket_bookings)
        {
            throw new NotImplementedException();
        }
    }
}
