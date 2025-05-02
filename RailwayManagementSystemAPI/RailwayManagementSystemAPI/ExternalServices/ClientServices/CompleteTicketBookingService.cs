using RailwayCore.Models;
using RailwayManagementSystemAPI.API_DTO;
using RailwayCore.Services;
using RailwayCore.InternalDTO.ModelDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    public class CompleteTicketBookingService
    {
        private readonly FullTicketBookingService ticket_booking_service;
        private readonly UserManagementService user_management_service;
        private const int timer_expiration = 1;
        public CompleteTicketBookingService(FullTicketBookingService ticket_booking_service, UserManagementService user_management_service)
        {
            this.ticket_booking_service = ticket_booking_service;
            this.user_management_service = user_management_service;
        }
        public async Task<MediatorTicketBookingDto?> InitializeTicketBookingProcess(InitialTicketBookingDto input)
        {
            QueryResult<User> user_authentication_result = await user_management_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return null;
            }
            User authenticated_user = user_authentication_result.Value!;
            InternalTicketBookingDtoWithCarriagePosition ticket_booking_dto_with_carriage_position = new InternalTicketBookingDtoWithCarriagePosition
            {
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Passenger_Carriage_Position_In_Squad = input.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = "********",
                Passenger_Surname = "********",
                User_Id = authenticated_user.Id,
                Ticket_Status = TicketStatus.Booking_In_Progress,
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title
            };
            QueryResult<TicketBooking> booking_result = await ticket_booking_service.CreateTicketBookingWithCarriagePositionInSquad(ticket_booking_dto_with_carriage_position);

            if (booking_result is FailQuery<TicketBooking>)
            {
                //API_ErrorHandler.AddError(ErrorHandler.GetLastErrorFromSingleService(ServiceName.TicketBookingService));
                return null;
            }
            TicketBooking ticket_booking = booking_result.Value!;
            _ = CancelTicketBookingAfterTimerExpiration(ticket_booking.Id);
            MediatorTicketBookingDto mediator_ticket_booking = new MediatorTicketBookingDto
            {
                Id = ticket_booking.Id,
                User_Id = ticket_booking.User_Id,
                Train_Route_On_Date_Id = ticket_booking.Train_Route_On_Date_Id,
                Passenger_Carriage_Position_In_Squad = input.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = ticket_booking.Place_In_Carriage,
                TicketStatus = GetTicketBookingStatusIntoString(ticket_booking.Ticket_Status),
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title,
                Booking_Initializing_Time = ticket_booking.Booking_Time,
                Booking_Expiration_Time = ticket_booking.Booking_Time.AddMinutes(timer_expiration),
                Passenger_Carriage_Id = ticket_booking.Passenger_Carriage_Id
            };
            return mediator_ticket_booking;
        }
        public async Task<CompletedTicketBookingDto?> CompleteTicketBookingProcess(MediatorTicketBookingDto input_unfinished_ticket,
            UserInfoForCompletedTicketBookingDto input_user_info)
        {
            TicketBooking? unfinished_ticket_booking = await ticket_booking_service
                .FindTicketBooking(input_unfinished_ticket.User_Id, input_unfinished_ticket.Train_Route_On_Date_Id, input_unfinished_ticket.Passenger_Carriage_Id,
                input_unfinished_ticket.Starting_Station_Title, input_unfinished_ticket.Ending_Station_Title, input_unfinished_ticket.Place_In_Carriage);
            if (unfinished_ticket_booking == null)
            {
                return null;
            }
            if (unfinished_ticket_booking.User_Id != input_user_info.User_Id)
            {
                return null;
            }
            if (DateTime.Now > input_unfinished_ticket.Booking_Expiration_Time)
            {
                return null;
            }
            unfinished_ticket_booking.Passenger_Name = input_user_info.Passenger_Name;
            unfinished_ticket_booking.Passenger_Surname = input_user_info.Passenger_Surname;
            unfinished_ticket_booking.Ticket_Status = TicketStatus.Booked_And_Active;
            unfinished_ticket_booking.Booking_Time = DateTime.Now;
            await ticket_booking_service.UpdateTicketBooking(unfinished_ticket_booking);
            return new CompletedTicketBookingDto
            {
                Id = unfinished_ticket_booking.Id,
                Passenger_Carriage_Id = unfinished_ticket_booking.Passenger_Carriage_Id,
                Passenger_Carriage_Position_In_Squad = input_unfinished_ticket.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = unfinished_ticket_booking.Place_In_Carriage,
                Starting_Station_Title = input_unfinished_ticket.Starting_Station_Title,
                Ending_Station_Title = input_unfinished_ticket.Ending_Station_Title,
                User_Id = input_unfinished_ticket.User_Id,
                Booking_Completion_Time = unfinished_ticket_booking.Booking_Time,
                Ticket_Status = GetTicketBookingStatusIntoString(unfinished_ticket_booking.Ticket_Status),
                Train_Route_On_Date_Id = unfinished_ticket_booking.Train_Route_On_Date_Id,
                Passenger_Name = unfinished_ticket_booking.Passenger_Name,
                Passenger_Surname = unfinished_ticket_booking.Passenger_Surname
            };
        }
        public string GetTicketBookingStatusIntoString(TicketStatus ticket_status)
        {
            switch (ticket_status)
            {
                case TicketStatus.Booking_In_Progress:
                    return "Booking_In_Progress";
                case TicketStatus.Booked_And_Active:
                    return "Booked_And_Active";
                case TicketStatus.Booked_And_Used:
                    return "Booked_And_Used";
                case TicketStatus.Returned:
                    return "Returned";
                case TicketStatus.Archieved:
                    return "Archieved";
                default:
                    return "";
            }
        }
        public async Task CancelTicketBookingAfterTimerExpiration(int ticket_booking_id)
        {
            await Task.Delay(timer_expiration * 60000);
            TicketBooking? ticket_booking = await ticket_booking_service.FindTicketBookingById(ticket_booking_id);
            if (ticket_booking == null)
            {
                return;
            }
            if (ticket_booking.Ticket_Status == TicketStatus.Booking_In_Progress)
            {
                await ticket_booking_service.DeleteTicketBooking(ticket_booking);
            }
        }
    }
}