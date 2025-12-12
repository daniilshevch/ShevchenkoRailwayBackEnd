using MailKit;
using MimeKit;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;
using System.Globalization;
using System.Net.Mail;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Implementations
{
    /// <summary>
    /// Даний сервіс проводить розсилку квитків після покупки, використовуючи Gmail API
    /// </summary>
    public class GmailSmtpTicketSender: IEmailTicketSender
    {
        private readonly string from_email;
        private readonly string app_password;
        private readonly string html_template;
        private readonly string service_title = "EmailTicketSender";
        private readonly IUserTicketManagementService user_ticket_management_service;
        private readonly IPdfTicketGeneratorService pdf_ticket_generator_service;
        private readonly IStationTranslator station_translator;
        private readonly ICarriageTypeTranslator carriage_type_translator;
        private readonly ITrainRoutesTranslator train_routes_translator;
        public GmailSmtpTicketSender(IConfiguration configuration, IUserTicketManagementService user_ticket_management_service,
            IPdfTicketGeneratorService pdf_ticket_generator_service, IStationTranslator station_translator,
            ICarriageTypeTranslator carriage_type_translator, ITrainRoutesTranslator train_routes_translator,
            IWebHostEnvironment web_host_environment)
        {
            this.from_email = configuration["GmailSmtp:Email"]!;
            this.app_password = configuration["GmailSmtp:AppPassword"]!;
            this.user_ticket_management_service = user_ticket_management_service;
            this.pdf_ticket_generator_service = pdf_ticket_generator_service;
            this.station_translator = station_translator;
            this.carriage_type_translator = carriage_type_translator;
            this.train_routes_translator = train_routes_translator;
            string template_path = Path.Combine(web_host_environment.ContentRootPath, "ExternalServices",
                "SystemServices", "EmailServices", "HtmlTemplate", "TicketEmailTemplate.html");
            if(File.Exists(template_path))
            {
                html_template = File.ReadAllText(template_path);
            }
            else
            {
                html_template = "<h1>Ticket for {Passenger_Name}</h1>";
            }
        }
        /// <summary>
        /// Даний метод відправляє один квиток на пошту користувача після покупки
        /// </summary>
        /// <param name="user_email"></param>
        /// <param name="ticket_booking_info"></param>
        /// <returns></returns>
        public async Task<QueryResult> SendTicketToEmailAsync(string user_email, TicketBooking ticket_booking_info)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Shevchenko Railway", from_email));
            message.To.Add(new MailboxAddress(ticket_booking_info.Passenger_Name, user_email));

            ExternalProfileTicketBookingDto ticket_booking_profile_for_pdf =
                await user_ticket_management_service.CreateProfileDtoForTicketBooking(ticket_booking_info);
            ticket_booking_profile_for_pdf.Full_Route_Starting_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Full_Route_Starting_Station_Title)!;
            ticket_booking_profile_for_pdf.Full_Route_Ending_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Full_Route_Ending_Station_Title)!;
            ticket_booking_profile_for_pdf.Trip_Starting_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Trip_Starting_Station_Title)!;
            ticket_booking_profile_for_pdf.Trip_Ending_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Trip_Ending_Station_Title)!;
            ticket_booking_profile_for_pdf.Carriage_Type = carriage_type_translator.TranslateCarriageTypeIntoUkrainian(ticket_booking_profile_for_pdf.Carriage_Type)!;
            ticket_booking_profile_for_pdf.Train_Route_Id = train_routes_translator.TranslateTrainRouteIdIntoUkrainian(ticket_booking_profile_for_pdf.Train_Route_Id)!;


            message.Subject = $"Квиток на поїзд {ticket_booking_profile_for_pdf.Train_Route_Id}, {ticket_booking_profile_for_pdf.Departure_Time_From_Trip_Starting_Station}";
 
            string logoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/00/Ukrzaliznytsia_logo.svg/1200px-Ukrzaliznytsia_logo.svg.png";

            BodyBuilder body_builder = new BodyBuilder();
            CultureInfo ua_culture = new CultureInfo("uk-UA");
            body_builder.HtmlBody = html_template
                    .Replace("{{Train_Route_Id}}", ticket_booking_profile_for_pdf.Train_Route_Id)
                    .Replace("{{Full_Route_Starting_Station_Title}}", ticket_booking_profile_for_pdf.Full_Route_Starting_Station_Title)
                    .Replace("{{Full_Route_Ending_Station_Title}}", ticket_booking_profile_for_pdf.Full_Route_Ending_Station_Title)
                    .Replace("{{Trip_Starting_Station_Title}}", ticket_booking_profile_for_pdf.Trip_Starting_Station_Title)
                    .Replace("{{Departure_Time_From_Trip_Starting_Station}}", ticket_booking_profile_for_pdf.Departure_Time_From_Trip_Starting_Station!.Value.ToString("dd MMMM о HH:mm", ua_culture))
                    .Replace("{{Trip_Ending_Station_Title}}", ticket_booking_profile_for_pdf.Trip_Ending_Station_Title)
                    .Replace("{{Arrival_Time_To_Trip_Ending_Station}}", ticket_booking_profile_for_pdf.Arrival_Time_To_Trip_Ending_Station!.Value.ToString("dd MMMM о HH:mm", ua_culture))
                    .Replace("{{Passenger_Name}}", ticket_booking_profile_for_pdf.Passenger_Name)
                    .Replace("{{Passenger_Surname}}", ticket_booking_profile_for_pdf.Passenger_Surname)
                    .Replace("{{Passenger_Carriage_Position_In_Squad}}", ticket_booking_info.Passenger_Carriage_Position_In_Squad.ToString())
                    .Replace("{{Carriage_Type}}", ticket_booking_profile_for_pdf.Carriage_Type)
                    .Replace("{{Carriage_Quality_Class}}", ticket_booking_profile_for_pdf.Carriage_Quality_Class)
                    .Replace("{{Place_In_Carriage}}", ticket_booking_profile_for_pdf.Place_In_Carriage.ToString())
                    .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            byte[] ticket_pdf = pdf_ticket_generator_service.GenerateTicketPdf(ticket_booking_profile_for_pdf);
            body_builder.Attachments.Add("Ticket.pdf", ticket_pdf, ContentType.Parse("application/pdf"));
            message.Body = body_builder.ToMessageBody();
            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.CheckCertificateRevocation = false;
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(from_email, app_password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    return new FailQuery(new Error(ErrorType.InternalServerError, $"Fail while attempt to send tickets to email: {user_email}. Error: {ex.Message}", 
                        annotation: service_title, unit: ProgramUnit.SystemAPI));
                }
            }
            return new SuccessQuery(new SuccessMessage($"Tickets have successfully been sent to email: {user_email}", annotation: service_title, unit: ProgramUnit.SystemAPI));
        }
    }
}
