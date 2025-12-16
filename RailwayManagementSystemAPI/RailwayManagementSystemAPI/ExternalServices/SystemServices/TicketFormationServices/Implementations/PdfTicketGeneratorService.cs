using Microsoft.EntityFrameworkCore.Query.Internal;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Implementations;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Implementations
{
    public class PdfTicketGeneratorService : IPdfTicketGeneratorService
    {
        private readonly IQRCodeGeneratorService qr_code_generator_service;
        private readonly IStationTranslator station_translator;
        private readonly ICarriageTypeTranslator carriage_type_translator;
        private readonly ITrainRoutesTranslator train_routes_translator;
        public PdfTicketGeneratorService(IQRCodeGeneratorService qr_code_generator_service, IStationTranslator station_translator,
            ICarriageTypeTranslator carriage_type_translator, ITrainRoutesTranslator train_routes_translator)
        {
            this.qr_code_generator_service = qr_code_generator_service;
            this.station_translator = station_translator;
            this.carriage_type_translator = carriage_type_translator;
            this.train_routes_translator = train_routes_translator;
            Settings.License = LicenseType.Community;
        }
        public byte[] GenerateTicketPdf(ExternalProfileTicketBookingDto ticket_booking)
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Налаштування сторінки під А5 (ландшафт)
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Element(container => _ConstructHeader(container, ticket_booking));
                    page.Content().Element(container => _ConstructContent(container, ticket_booking));
                    page.Footer().Element(container => _ConstructFooter(container, ticket_booking));
                });
            });

            return document.GeneratePdf();
        }
        public void TranslateTicketIntoUkrainian(ExternalProfileTicketBookingDto ticket_booking_profile_for_pdf)
        {
            ticket_booking_profile_for_pdf.Full_Route_Starting_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Full_Route_Starting_Station_Title)!;
            ticket_booking_profile_for_pdf.Full_Route_Ending_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Full_Route_Ending_Station_Title)!;
            ticket_booking_profile_for_pdf.Trip_Starting_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Trip_Starting_Station_Title)!;
            ticket_booking_profile_for_pdf.Trip_Ending_Station_Title = station_translator.TranslateStationTitleIntoUkrainian(ticket_booking_profile_for_pdf.Trip_Ending_Station_Title)!;
            ticket_booking_profile_for_pdf.Carriage_Type = carriage_type_translator.TranslateCarriageTypeIntoUkrainian(ticket_booking_profile_for_pdf.Carriage_Type)!;
            ticket_booking_profile_for_pdf.Train_Route_Id = train_routes_translator.TranslateTrainRouteIdIntoUkrainian(ticket_booking_profile_for_pdf.Train_Route_Id)!;
        }
        public void _ConstructHeader(IContainer container, ExternalProfileTicketBookingDto ticket_booking)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("ООО «SHEVCHENKO RAILWAY»").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                    col.Item().Text("ЕЛЕКТРОННИЙ ПОСАДКОВИЙ ДОКУМЕНТ").FontSize(8).FontColor(Colors.Grey.Medium);

                    if (ticket_booking.Full_Ticket_Id.HasValue)
                    {
                        col.Item().Text($"ID КВИТКА: {ticket_booking.Full_Ticket_Id}").FontSize(7).FontColor(Colors.Grey.Darken1);
                    }
                });
                // row.ConstantItem(50).Image("logo.png"); 
            });
        }
        public void _ConstructContent(IContainer container, ExternalProfileTicketBookingDto ticket_booking)
        {
            container.PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().PaddingRight(15).Column(col =>
                {
                    col.Item().Text("ПАСАЖИР / PASSENGER").FontSize(8).FontColor(Colors.Grey.Medium);
                    col.Item().Text($"{ticket_booking.Passenger_Surname} {ticket_booking.Passenger_Name}".ToUpper())
                       .Bold().FontSize(16);

                    col.Spacing(10);
                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Spacing(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(defs =>
                        {
                            defs.RelativeColumn();
                            defs.ConstantColumn(20);
                            defs.RelativeColumn();
                        });

                        table.Cell().Column(c =>
                        {
                            c.Item().Text("ВІДПРАВЛЕННЯ").FontSize(8).FontColor(Colors.Grey.Medium);
                            c.Item().Text(ticket_booking.Trip_Starting_Station_Title).Bold().FontSize(11);
                            c.Item().Text($"{ticket_booking.Departure_Time_From_Trip_Starting_Station:dd.MM.yyyy HH:mm}").FontSize(12).Bold();
                        });

                        table.Cell().AlignCenter().AlignMiddle().Text("→").FontSize(14).FontColor(Colors.Grey.Medium);

                        table.Cell().Column(c =>
                        {
                            c.Item().Text("ПРИБУТТЯ").FontSize(8).FontColor(Colors.Grey.Medium);
                            c.Item().Text(ticket_booking.Trip_Ending_Station_Title).Bold().FontSize(11);
                            c.Item().Text($"{ticket_booking.Arrival_Time_To_Trip_Ending_Station:dd.MM.yyyy HH:mm}").FontSize(12).Bold();
                        });
                    });

                    col.Spacing(10);

                    col.Item().Background(Colors.Grey.Lighten4).Padding(5).Table(table =>
                    {
                        table.ColumnsDefinition(defs =>
                        {
                            defs.RelativeColumn();
                            defs.RelativeColumn();
                            defs.RelativeColumn();
                            defs.RelativeColumn();
                        });

                        table.Cell().Text("ПОЇЗД").FontSize(7).FontColor(Colors.Grey.Darken2);
                        table.Cell().Text("ВАГОН").FontSize(7).FontColor(Colors.Grey.Darken2);
                        table.Cell().Text("МІСЦЕ").FontSize(7).FontColor(Colors.Grey.Darken2);
                        table.Cell().Text("КЛАС").FontSize(7).FontColor(Colors.Grey.Darken2);

                        table.Cell().Text(ticket_booking.Train_Route_Id).Bold().FontSize(12);
                        table.Cell().Text(ticket_booking.Passenger_Carriage_Position_In_Squad?.ToString() ?? "-").Bold().FontSize(12);
                        table.Cell().Text(ticket_booking.Place_In_Carriage.ToString()).Bold().FontSize(12);

                        string carClass = $"{ticket_booking.Carriage_Type} {ticket_booking.Carriage_Quality_Class ?? ""}".Trim();
                        table.Cell().Text(carClass).FontSize(10);
                    });
                });

                row.ConstantItem(130).Column(col =>
                {
                    col.Item().AlignCenter().Text("СКАНИЙ ТУТ").FontSize(8).FontColor(Colors.Grey.Medium);

                    if(ticket_booking.Qr_Code is null)
                    {
                        ticket_booking.Qr_Code = qr_code_generator_service.GenerateQrCodeBase64(ticket_booking.Full_Ticket_Id.ToString()!);
                    }
                    // Обробка QR коду з Base64
                    byte[] qrBytes = _GetQrImageBytes(ticket_booking.Qr_Code);
                    if (qrBytes.Length > 0)
                    {
                        col.Item().Height(130).Width(130).Image(qrBytes);
                    }
                    else
                    {
                        col.Item().Height(130).Width(130).Placeholder("QR Error");
                    }

                    col.Spacing(5);
                    col.Item().Border(1).BorderColor(Colors.Black).Padding(3).AlignCenter()
                        .Text(ticket_booking.Ticket_Status.ToUpper())
                        .Bold().FontSize(10);
                });
            });
        }
        public void _ConstructFooter(IContainer container, ExternalProfileTicketBookingDto ticket_booking)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                col.Item().PaddingTop(3).Row(row =>
                {
                    row.RelativeItem().Text("Цей документ є підставою для проїзду.").FontSize(7);

                    if (ticket_booking.Trip_Duration.HasValue)
                        row.AutoItem().Text($"Час у дорозі: {ticket_booking.Trip_Duration.Value:hh\\:mm}").FontSize(8).Bold();
                });
            });
        }
        public byte[] _GetQrImageBytes(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return Array.Empty<byte>();

            try
            {
                // Твій сервіс повертає рядок виду "data:image/png;base64,iVBORw0KGgoAAA..."
                // Нам треба відкинути все до коми.
                var parts = base64String.Split(',');
                var cleanBase64 = parts.Length > 1 ? parts[1] : parts[0];

                return Convert.FromBase64String(cleanBase64);
            }
            catch
            {
                // Логування помилки можна додати тут
                return Array.Empty<byte>();
            }
        }

    }
}
