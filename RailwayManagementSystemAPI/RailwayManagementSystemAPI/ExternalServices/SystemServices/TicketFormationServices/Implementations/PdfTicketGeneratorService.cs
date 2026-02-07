using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Implementations
{
    public class PdfTicketGeneratorService : IPdfTicketGeneratorService
    {
        private readonly IQRCodeGeneratorService qr_code_generator_service;
        private readonly IStationTranslator station_translator;
        private readonly ICarriageTypeTranslator carriage_type_translator;
        private readonly ITrainRoutesTranslator train_routes_translator;
        private readonly ITicketStatusTranslator ticket_status_translator;
        public PdfTicketGeneratorService(IQRCodeGeneratorService qr_code_generator_service, IStationTranslator station_translator,
            ICarriageTypeTranslator carriage_type_translator, ITrainRoutesTranslator train_routes_translator, ITicketStatusTranslator ticket_status_translator)
        {
            this.qr_code_generator_service = qr_code_generator_service;
            this.station_translator = station_translator;
            this.carriage_type_translator = carriage_type_translator;
            this.train_routes_translator = train_routes_translator;
            this.ticket_status_translator = ticket_status_translator;
            Settings.License = LicenseType.Community;
        }
        public byte[] GenerateTicketPdf(ExternalProfileTicketBookingDto ticket_booking)
        {
            if (string.IsNullOrEmpty(ticket_booking.Qr_Code) && ticket_booking.Full_Ticket_Id is not null)
            {
                ticket_booking.Qr_Code = qr_code_generator_service.GenerateQrCodeBase64(ticket_booking.Full_Ticket_Id.ToString());
            }
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana).FontColor(Colors.Black));

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
            ticket_booking_profile_for_pdf.Ticket_Status = ticket_status_translator.TranslateTicketStatusIntoUkrainian(ticket_booking_profile_for_pdf.Ticket_Status)!;
        }
        public void _ConstructHeader(IContainer container, ExternalProfileTicketBookingDto ticket_booking)
        {
            container.PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("SHEVCHENKO RAILWAY").ExtraBold().FontSize(22).FontColor(Colors.Blue.Medium);
                    column.Item().Text("ЕЛЕКТРОННИЙ ПОСАДКОВИЙ ДОКУМЕНТ").SemiBold().FontSize(9).FontColor(Colors.Grey.Darken2);
                });

                row.RelativeItem().AlignRight().Column(column =>
                {
                    column.Item().Text("КВИТОК ДІЙСНИЙ").ExtraBold().FontSize(10).FontColor(Colors.Green.Medium);
                    column.Item().Text(DateTime.Now.ToString("g")).FontSize(8).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(1).Text($"ID: {ticket_booking.Full_Ticket_Id}").FontSize(7).FontColor(Colors.Grey.Lighten1);
                });
            });
        
        }
        public void _ConstructContent(IContainer container, ExternalProfileTicketBookingDto ticket_booking)
        {
            string quality_class = ticket_booking.Carriage_Quality_Class?.ToUpper() ?? "";
            QuestPDF.Infrastructure.Color class_color = quality_class switch
            {
                "S" => Colors.Purple.Medium,
                "A" => Colors.Red.Medium,
                "B" => Colors.Green.Medium,
                "C" => Colors.Blue.Medium,
                _ => Colors.Black
            };

            container.Column(main_column =>
            {
                main_column.Item().Row(row =>
                {
                    row.ConstantItem(170).BorderRight(1).BorderColor(Colors.Grey.Lighten3).PaddingRight(15).Column(column =>
                    {
                        byte[] qr_bytes = _GetQrImageBytes(ticket_booking.Qr_Code);
                        column.Item().Width(155).Height(155)
                            .Border(1f).BorderColor(Colors.Grey.Lighten3)
                            .Padding(2)
                            .Image(qr_bytes);

                        column.Spacing(10);

                        column.Item().AlignCenter().Row(statusRow =>
                        {
                            statusRow.AutoItem()
                                .Border(1)
                                .BorderColor(Colors.Blue.Medium)
                                .Background(Colors.Blue.Lighten5) 
                                .PaddingHorizontal(10)
                                .PaddingVertical(2)
                                .Text(ticket_booking.Ticket_Status?.ToUpper() ?? "ACTIVE")
                                .ExtraBold()
                                .FontSize(9)
                                .FontColor(Colors.Blue.Medium);
                        });
                    });

                    row.RelativeItem().PaddingLeft(15).Column(column =>
                    {
                        column.Item().Text("ПАСАЖИР / PASSENGER").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                        column.Item().PaddingBottom(10).Text($"{ticket_booking.Passenger_Surname} {ticket_booking.Passenger_Name}".ToUpper()).ExtraBold().FontSize(20);

                        column.Item().LineHorizontal(2f).LineColor(class_color);

                        column.Item().PaddingVertical(15).Row(route_row =>
                        {
                            route_row.RelativeItem().Column(c => _BuildStationBlock(c, "ВІДПРАВЛЕННЯ", ticket_booking.Trip_Starting_Station_Title, ticket_booking.Departure_Time_From_Trip_Starting_Station, class_color));
                            route_row.RelativeItem().Column(c => _BuildStationBlock(c, "ПРИБУТТЯ", ticket_booking.Trip_Ending_Station_Title, ticket_booking.Arrival_Time_To_Trip_Ending_Station, class_color));
                        });

                        column.Item().Background(Colors.Grey.Lighten4).Padding(8).Row(info_row =>
                        {
                            info_row.AutoItem().MinWidth(80).Column(column => _BuildInfoCell(column, "ПОЇЗД", ticket_booking.Train_Route_Id));
                            info_row.AutoItem().MinWidth(80).Column(column => _BuildInfoCell(column, "ВАГОН", ticket_booking.Passenger_Carriage_Position_In_Squad?.ToString() ?? "-"));
                            info_row.AutoItem().MinWidth(80).Column(column => _BuildInfoCell(column, "МІСЦЕ", ticket_booking.Place_In_Carriage.ToString()));

                            info_row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("ТИП, КЛАС").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                                column.Item().PaddingTop(2).Text(text =>
                                {
                                    text.Span(ticket_booking.Carriage_Type?.ToUpper() + "," ?? "СТАНДАРТ").ExtraBold().FontSize(12);

                                    if (!string.IsNullOrEmpty(quality_class))
                                    {
                                        text.Span($" {quality_class}").ExtraBold().FontSize(12).FontColor(class_color);
                                    }
                                });
                            });
                        });
                    });
                });
            });
        }
        private void _BuildStationBlock(ColumnDescriptor col, string label, string station, DateTime? time, QuestPDF.Infrastructure.Color class_color)
        {
            col.Item().Text(label).FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
            col.Item().PaddingTop(2).Text(station ?? "---").ExtraBold().FontSize(14);
            col.Item().PaddingTop(2).Text(time?.ToString("dd.MM.yyyy") ?? "--.--.----").SemiBold().FontSize(10);
            col.Item().Text(time?.ToString("HH:mm") ?? "--:--").ExtraBold().FontSize(24).FontColor(class_color);
        
        }
        private void _BuildInfoCell(ColumnDescriptor col, string label, string value)
        {
            col.Item().Text(label).FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
            col.Item().PaddingTop(2).Text(value).ExtraBold().FontSize(12);
        }
        private void _ConstructFooter(IContainer container, ExternalProfileTicketBookingDto ticket_booking)
        {
            container.PaddingTop(15).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Зберігайте квиток до кінця поїздки. Документ підтверджує право на проїзд та страхування.").FontSize(7).Italic();
                    col.Item().Text("Залізниця бажає вам приємної подорожі!").FontSize(7).SemiBold();
                });

                if (ticket_booking.Trip_Duration.HasValue)
                {
                    row.AutoItem().AlignBottom().Text(x =>
                    {
                        x.Span("ЧАС У ДОРОЗІ: ").FontSize(9).SemiBold();
                        x.Span($"{ticket_booking.Trip_Duration.Value:hh\\:mm}").FontSize(11).ExtraBold();
                    });
                }
            });
        }
 
        public byte[] _GetQrImageBytes(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return Array.Empty<byte>();

            try
            {
                var parts = base64String.Split(',');
                var cleanBase64 = parts.Length > 1 ? parts[1] : parts[0];
                return Convert.FromBase64String(cleanBase64);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

    }
}


