using MailKit;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MimeKit;
using Org.BouncyCastle.Crypto;
using RailwayCore.InternalServices.CoreServices.Interfaces;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;
using System.Collections.Generic;
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
        private readonly IFullTicketManagementService full_ticket_management_service;
        private readonly IPdfTicketGeneratorService pdf_ticket_generator_service;
        public GmailSmtpTicketSender(IConfiguration configuration, IUserTicketManagementService user_ticket_management_service,
            IPdfTicketGeneratorService pdf_ticket_generator_service, IFullTicketManagementService full_ticket_management_service,
            IWebHostEnvironment web_host_environment)
        {
            this.from_email = configuration["GmailSmtp:Email"]!;
            this.app_password = configuration["GmailSmtp:AppPassword"]!;
            this.user_ticket_management_service = user_ticket_management_service;
            this.pdf_ticket_generator_service = pdf_ticket_generator_service;
            this.full_ticket_management_service = full_ticket_management_service;
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
            ExternalProfileTicketBookingDto ticket_booking_profile_for_pdf =
                await user_ticket_management_service.CreateProfileDtoForTicketBooking(ticket_booking_info);
            pdf_ticket_generator_service.TranslateTicketIntoUkrainian(ticket_booking_profile_for_pdf);
            return await _SendOrganisedTicketBookingToEmail(user_email, ticket_booking_profile_for_pdf, ticket_booking_info);
        }

        public async Task<QueryResult> SendMultipleTicketsToEmail(string user_email, List<ExternalOutputCompletedTicketBookingDto> ticket_bookings_list)
        {
            List<TicketBooking> ticket_bookings = await full_ticket_management_service.FindSeveralTicketBookingsById(ticket_bookings_list.Select(ticket_booking => ticket_booking.Id).ToList());
            List<Task<QueryResult>> tickets_sending_to_email_task_list = new List<Task<QueryResult>>();
            foreach(TicketBooking ticket_booking in ticket_bookings)
            {
                ExternalProfileTicketBookingDto profile_ticket_booking_dto = await user_ticket_management_service.CreateProfileDtoForTicketBooking(ticket_booking);
                pdf_ticket_generator_service.TranslateTicketIntoUkrainian(profile_ticket_booking_dto);
                tickets_sending_to_email_task_list.Add(_SendOrganisedTicketBookingToEmail(user_email, profile_ticket_booking_dto, ticket_booking));
            }
            IEnumerable<QueryResult> tickets_send_result = await Task.WhenAll(tickets_sending_to_email_task_list);
            foreach (QueryResult single_ticket_send_result in tickets_send_result)
            {
                if (single_ticket_send_result.Fail)
                {
                    return new FailQuery(single_ticket_send_result.Error);
                }
            }
            return new SuccessQuery(new SuccessMessage($"All tickets have been successfully sent to {user_email}", annotation: service_title, unit: ProgramUnit.ClientAPI));
        }
        public async Task<QueryResult> _SendOrganisedTicketBookingToEmail(string user_email, ExternalProfileTicketBookingDto ticket_booking_profile_for_pdf, TicketBooking ticket_booking_info)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Shevchenko Railway", from_email));
            message.To.Add(new MailboxAddress(ticket_booking_info.Passenger_Name, user_email));
            string logoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/00/Ukrzaliznytsia_logo.svg/1200px-Ukrzaliznytsia_logo.svg.png";
            //string logo_Url = "https://lh3.googleusercontent.com/rd-gg-dl/ABS2GSlAGdqMXm-2U_hW0TUXsffvIIkmWjbx_bkLMLsb-m7MsNin67267dzvBzkGcMkkQneTnbRKfkaDzQ1YwDxwItIfHy1usbOmB_zBbBT_fmoxXJGLYUU_oCiDm1Agy15Y-LQKfNORTpO5T_Ie_MUGvYhPD3OA_ySACXxwoqjb2XkFHsyFqcechgOl8WTmOf7tbs7Hffs5wNmozspV60c1ZyTVC9tOLo9I8gWVQiyK2cWH3hMShFyqoZoIUIn5K7R41CErBp5UGj4LxuULEG_XuJoM5D6mg3NIIldArrb9Z5nRUySkfJY9sTqU-LUkW5YeDLF5lUteyhMp0KILWWzYClELv_6nR7Syvg-35siiVlRgYphldI91OLODkhHE_fHqp2jETWBZK0r9V1N1jzsq-sAbcQIaXy-iqlKe1_srk0T6RZ_Zrx8Z8zbWaF8XspBM2eW8EB50dLWPwBFEbh5tyrSBKBUVzlUTMUL8VNu6eDNw1My3WVpPicQ7Ov1l3XDHbZB_OsATXH_4hmh2TqawYN-0hH8EYx6r1ERu_oJPANBiwntARLixARzX2p5UR5eGi1KWMuN4ZCkWTP2jrFxi7eyhUEEBarylPNMfH8TeC_W2fsXoD4V1mrdc3DhSNw7rqlpoF_QanhN5xThKKyUa5PNzWN5VfD6gQ1qfqbsUVoRl9FIbLpXuXRs_sRhxWCMQRDMScqWOIF6LrwgX1rdkwI3-21dP13qLTmsJJoRYEC-d_N-QXNc8OxXYPiw0Ooe4jQz54WnKhcJRDt6YI2Of89lH6Yf5kGOU8XNT4vzTZzK1jxPGFhstoSi7_AE6AceGe5nnaX-3RjVVP7bgaQc8v1PwcyBslljiHifOlVZxs2YVnRPXmxGqW_TzswGJyyZXObijPNIBqeyg3DGbiO4cQKQU86ohbSL_wub_yhNs0_VLs_QmCQcBOBVZcXld1HG4JqZ0b-Vk8YcIXXnXks2b3aXx4f6btT6t5nErT9Ehto4H3t4SRzZTDQnvca1Ks3bFgpOUL_jBQofroikI86-P7cLAFSdhyxZd7qu48Dr8EGHqNy9ODI-XGOjLq1WmDJ-S6UWv9sTm4A_ygzpLeHJqEFJaWo3gKR1ikRQEFAu4U98iKMnaA29X8OqWFjJAcrs77926w3Nrn1krm6m2kkoEKvyifWMrNiApAST9BSMkbWJGBFUv973Bd3v7BXp7afUUYPoYpUVFfhbxCX2RpPuHcf_AeHm8xQE0sltDOP7z1cljI8URwnvE9W6hoU4duFjKUPZgbm5UxcgDnE98ad-CglWML8m0huLUYcVw3HMSHKIOc1Sq-SCxa-RxFZBhOt5G74iatt-2FNGGCAIqVfjsNlzGbEvhxxXfs3tfqOgDC9HGkv78LulVJ1mfrqXfQxfM_D1bW6gu-uE_gdA=s1024-rj?authuser=2";
            message.Subject = $"Квиток на поїзд {ticket_booking_profile_for_pdf.Train_Route_Id}, {ticket_booking_profile_for_pdf.Departure_Time_From_Trip_Starting_Station}";
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
            return new SuccessQuery(new SuccessMessage($"Ticket {ticket_booking_info.Full_Ticket_Id} has successfully been sent to email: {user_email}", annotation: service_title, unit: ProgramUnit.SystemAPI));
        }
        public async Task<QueryResult> SendMultipleTicketBookingsInGroupsToEmail(string user_email, List<ExternalOutputCompletedTicketBookingDto> ticket_bookings_list)
        {
            List<TicketBooking> ticket_bookings = await full_ticket_management_service.FindSeveralTicketBookingsById(ticket_bookings_list.Select(ticket_booking => ticket_booking.Id).ToList());
            List<(ExternalProfileTicketBookingDto Profile, TicketBooking Info)> ticket_bookings_data_list = new List<(ExternalProfileTicketBookingDto, TicketBooking)>();
            foreach (TicketBooking ticket_booking in ticket_bookings)
            {
                ExternalProfileTicketBookingDto profile_ticket_booking_dto = await user_ticket_management_service.CreateProfileDtoForTicketBooking(ticket_booking);
                pdf_ticket_generator_service.TranslateTicketIntoUkrainian(profile_ticket_booking_dto);
                ticket_bookings_data_list.Add((profile_ticket_booking_dto, ticket_booking));
            }

            var ticket_groups = ticket_bookings_data_list.GroupBy(ticket => new
            {
                ticket.Profile.Train_Route_On_Date_Id,
                ticket.Profile.Trip_Starting_Station_Title,
                ticket.Profile.Trip_Ending_Station_Title
            });

            List<Task<QueryResult>> ticket_sending_to_email_task_list = new List<Task<QueryResult>>();
            foreach (var ticket_group in ticket_groups)
            {
                ticket_sending_to_email_task_list.Add(_SendGroupOfTicketsToEmail(user_email, ticket_group.ToList()));
            }
            IEnumerable<QueryResult> ticket_send_results = await Task.WhenAll(ticket_sending_to_email_task_list);
            foreach (QueryResult query_result in ticket_send_results)
            {
                if(query_result.Fail)
                {
                    return query_result;
                }
            }
            return new SuccessQuery(new SuccessMessage($"All tickets have been successfully sent to email: {user_email}", annotation: service_title, unit: ProgramUnit.SystemAPI));

        }
        public async Task<QueryResult> _SendGroupOfTicketsToEmail(string user_email, List<(ExternalProfileTicketBookingDto, TicketBooking)> ticket_booking_group)
        {
            (ExternalProfileTicketBookingDto Profile, TicketBooking Info) first_ticket_booking = ticket_booking_group[0];
            ExternalProfileTicketBookingDto first_ticket_booking_profile = first_ticket_booking.Profile;
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Shevchenko Railway", from_email));
            message.To.Add(new MailboxAddress(first_ticket_booking_profile.Passenger_Name, user_email));
            string subject_starting_word = ticket_booking_group.Count > 1 ? "Квитки" : "Квиток"; 
            message.Subject = $"{subject_starting_word} на поїзд {first_ticket_booking_profile.Train_Route_Id}, {first_ticket_booking_profile.Departure_Time_From_Trip_Starting_Station}";
            BodyBuilder body_builder = new BodyBuilder();
            CultureInfo ua_culture = new CultureInfo("uk-UA");

            string passengers_html = "";
            foreach((ExternalProfileTicketBookingDto Profile, TicketBooking Info) single_ticket in ticket_booking_group)
            {
                byte[] ticket_pdf = pdf_ticket_generator_service.GenerateTicketPdf(single_ticket.Profile);
                body_builder.Attachments.Add($"Ticket_{single_ticket.Profile.Passenger_Surname}_{single_ticket.Info.Full_Ticket_Id}.pdf", ticket_pdf, ContentType.Parse("application/pdf"));
                passengers_html += $@"
            <tr>
                <td class='value' style='width: 50%; white-space: nowrap; padding: 12px 20px; border-top: 1px solid #eee;'>
                    {single_ticket.Profile.Passenger_Name} {single_ticket.Profile.Passenger_Surname}
                </td>
                <td class='value' style='text-align: right; padding: 12px 20px; border-top: 1px solid #eee;'>
                    Вагон {single_ticket.Info.Passenger_Carriage_Position_In_Squad} ({single_ticket.Profile.Carriage_Type}, клас 
                    <span class='quality-{single_ticket.Profile.Carriage_Quality_Class}'>{single_ticket.Profile.Carriage_Quality_Class}</span>)
                    <br>
                    <span style='font-size: 15px; display: inline-block; margin-top: 4px;'>місце {single_ticket.Profile.Place_In_Carriage}</span>
                </td>
            </tr>";
            }

            body_builder.HtmlBody = html_template
                .Replace("{{Train_Route_Id}}", first_ticket_booking_profile.Train_Route_Id)
                .Replace("{{Full_Route_Starting_Station_Title}}", first_ticket_booking_profile.Full_Route_Starting_Station_Title)
                .Replace("{{Full_Route_Ending_Station_Title}}", first_ticket_booking_profile.Full_Route_Ending_Station_Title)
                .Replace("{{Trip_Starting_Station_Title}}", first_ticket_booking_profile.Trip_Starting_Station_Title)
                .Replace("{{Departure_Time_From_Trip_Starting_Station}}", first_ticket_booking_profile.Departure_Time_From_Trip_Starting_Station!.Value.ToString("dd MMMM о HH:mm", ua_culture))
                .Replace("{{Trip_Ending_Station_Title}}", first_ticket_booking_profile.Trip_Ending_Station_Title)
                .Replace("{{Arrival_Time_To_Trip_Ending_Station}}", first_ticket_booking_profile.Arrival_Time_To_Trip_Ending_Station!.Value.ToString("dd MMMM о HH:mm", ua_culture))
                .Replace("{{PASSENGERS_BLOCK}}", passengers_html) 
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

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
                    return new FailQuery(new Error(ErrorType.InternalServerError, $"Fail while attempt to send groupd of tickets to email: {user_email}. Error: {ex.Message}",
                        annotation: service_title, unit: ProgramUnit.SystemAPI));
                }
            }
            return new SuccessQuery(new SuccessMessage($"Group of tickets has been successfully sent to email: {user_email}", annotation: service_title, unit: ProgramUnit.SystemAPI));

        }
       
    }
}
