using RailwayCore.Models;
using RailwayManagementSystemAPI.API_DTO;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RailwayCore.Migrations;
using Newtonsoft.Json;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    public class CompleteTicketBookingProcessingService
    {
        private readonly FullTicketManagementService full_ticket_management_service;
        private readonly SystemAuthenticationService system_authentication_service;
        private readonly IConfiguration configuration;
        public CompleteTicketBookingProcessingService(FullTicketManagementService full_ticket_management_service, SystemAuthenticationService system_authentication_service, IConfiguration configuration)
        {
            this.full_ticket_management_service = full_ticket_management_service;
            this.system_authentication_service = system_authentication_service;
            this.configuration = configuration;
        }
        [Refactored("v1", "02.05.2025")]
        public async Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcessForUser
    (ExternalInputInitialTicketBookingDto input, User user)
        {
            //Робимо ініціалізацію квитка, виставляючи всі дані бажаної подорожі і прив'язуючи квиток до акаунту, але не вказуємо ім'я пасажира
            //Статус квитка - Booking_In_Progress
            InternalTicketBookingDtoWithCarriagePosition internal_ticket_dto = new InternalTicketBookingDtoWithCarriagePosition
            {
                User_Id = user.Id,
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Passenger_Carriage_Position_In_Squad = input.Passenger_Carriage_Position_In_Squad,
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = "********",
                Passenger_Surname = "********",
                Ticket_Status = TicketStatus.Booking_In_Progress,
            };
            //Створюємо квиток через внутрішній сервіс(RailwayCore)
            QueryResult<TicketBooking> ticket_booking_result = await full_ticket_management_service.CreateTicketBookingWithCarriagePositionInSquad(internal_ticket_dto);
            if (ticket_booking_result.Fail)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(ticket_booking_result.Error);
            }
            TicketBooking current_ticket_booking = ticket_booking_result.Value;
            //Додаємо квитку інформацію про те, коли буде спливати бронь(актуально, адже квиток перебуває в статусі бронювання)
            current_ticket_booking.Booking_Expiration_Time = DateTime.Now.AddMinutes(Convert.ToDouble(configuration["RailwaySystemFunctioning:BookingExpirationTimeInMinutes"]));
            await full_ticket_management_service.UpdateTicketBooking(current_ticket_booking);
            ///////////////Sockets
            await WebSocketHandler.BroadcastAsync(JsonConvert.SerializeObject(new
            {
                train_route_id = input.Train_Route_On_Date_Id,
                carriage_position = input.Passenger_Carriage_Position_In_Squad,
                place_in_carriage = input.Place_In_Carriage,
                is_free = false
            }));
            /////////////////////
            //Видаємо проміжну інформацію про квиток на даний момент(бронювання вже ініціалізоване і перебуває в процесі)
            ExternalOutputMediatorTicketBookingDto mediator_ticket_booking_dto = new ExternalOutputMediatorTicketBookingDto
            {
                Id = current_ticket_booking.Id,
                User_Id = current_ticket_booking.User_Id,
                Train_Route_On_Date_Id = current_ticket_booking.Train_Route_On_Date_Id,
                Passenger_Carriage_Id = current_ticket_booking.Passenger_Carriage_Id,
                Passenger_Carriage_Position_In_Squad = current_ticket_booking.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = current_ticket_booking.Place_In_Carriage,
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title,
                Booking_Initializing_Time = current_ticket_booking.Booking_Time,
                Booking_Expiration_Time = current_ticket_booking.Booking_Expiration_Time,
                TicketStatus = TextEnumConvertationService.GetTicketBookingStatusIntoString(current_ticket_booking.Ticket_Status)
            };
            return new SuccessQuery<ExternalOutputMediatorTicketBookingDto>(mediator_ticket_booking_dto);

        }
        public async Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcessForAuthenticatedUser
            (ExternalInputInitialTicketBookingDto input)
        {
            //Отримуємо аутентифікованого користувача
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            return await InitializeTicketBookingProcessForUser(input, user);
        }
        public async Task<QueryResult<List<ExternalOutputMediatorTicketBookingDto>>> InitializeMultipleTicketBookingProcessForAuthenticatedUser(
            List<ExternalInputInitialTicketBookingDto> ticket_bookings_list)
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalOutputMediatorTicketBookingDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            List<ExternalOutputMediatorTicketBookingDto> ticket_bookings = new List<ExternalOutputMediatorTicketBookingDto>();
            foreach(ExternalInputInitialTicketBookingDto ticket_booking_dto in ticket_bookings_list)
            {
                QueryResult<ExternalOutputMediatorTicketBookingDto> ticket_booking_result = 
                    await InitializeTicketBookingProcessForUser(ticket_booking_dto, user);
                if(ticket_booking_result.Fail)
                {
                    ticket_bookings.Add(new ExternalOutputMediatorTicketBookingDto()
                    {
                        User_Id = user.Id,
                        Train_Route_On_Date_Id = ticket_booking_dto.Train_Route_On_Date_Id,
                        Passenger_Carriage_Position_In_Squad = ticket_booking_dto.Passenger_Carriage_Position_In_Squad,
                        Place_In_Carriage = ticket_booking_dto.Place_In_Carriage,
                        Starting_Station_Title = ticket_booking_dto.Starting_Station_Title,
                        Ending_Station_Title = ticket_booking_dto.Ending_Station_Title,
                        TicketStatus = TextEnumConvertationService.GetTicketBookingStatusIntoString(TicketStatus.Booking_Failed)
                    });
                }
                else
                {
                    ExternalOutputMediatorTicketBookingDto successfully_booked_ticket = ticket_booking_result.Value;
                    ticket_bookings.Add(successfully_booked_ticket);
                }
            }
            return new SuccessQuery<List<ExternalOutputMediatorTicketBookingDto>>(ticket_bookings);
        }
        /*
        public async Task<QueryResult<ExternalOutputMediatorTicketBookingDto> CancelTicketBookingProcessForAuthenticatedUser
            (ExternalOutputMediatorTicketBookingDto input_unfinished_ticket)
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if(user_authentication_result.Fail)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            TicketBooking? ticket_booking = await full_ticket_management_service.FindTicketBookingById(input_unfinished_ticket.Id);
            if(ticket_booking is null)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.NotFound, "System can't find the ticket to cancel booking"));
            }
            if(user.Id != input_unfinished_ticket.User_Id)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.Forbidden, "Authenticated user doesn't own the ticket whose booking is being tried to be completed"));
            }
            if(ticket_booking.Ticket_Status != TicketStatus.Booking_In_Progress)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.Forbidden, "Ticket booking proccess has already been completed"));
            }

        }*/

        [Refactored("v1", "02.05.2025")]
        public async Task<QueryResult<ExternalOutputCompletedTicketBookingDto>> CompleteTicketBookingProcessForAuthenticatedUser
            (ExternalOutputMediatorTicketBookingDto input_unfinished_ticket, ExternalInputPassengerInfoForCompletedTicketBookingDto passenger_info)
        {
            //Отримуємо аутентифікованого користувача
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            //Отримуємо з бази запис про поточне бронювання
            TicketBooking? ticket_booking = await full_ticket_management_service.FindTicketBookingById(input_unfinished_ticket.Id);
            //Перевірка, чи квиток є в базі
            if (ticket_booking is null)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.InternalServerError, "System can't find the ticket to complete booking"));
            }
            //Перевірка, чи аутентифікований користувач є тим самим, який розпочав процес бронювання квитка
            if (user.Id != input_unfinished_ticket.User_Id)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.Forbidden, "Authenticated user doesn't own the ticket whose booking is being tried to be completed"));
            }
            //Перевірка,чи не сплив час бронювання
            if (DateTime.Now > ticket_booking.Booking_Expiration_Time)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.Forbidden, "Booking reservation time has expired"));
            }
            if (ticket_booking.Ticket_Status != TicketStatus.Booking_In_Progress)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.BadRequest, "This place has already been booked"));
            }
            //Встановлюємо інформацію про пасажира
            ticket_booking.Passenger_Name = passenger_info.Passenger_Name;
            ticket_booking.Passenger_Surname = passenger_info.Passenger_Surname;
            //Оновлюємо час бронювання
            ticket_booking.Booking_Time = DateTime.Now;
            //Видаляємо інформацію про час закінчення періоду бронювання(адже бронювання завершено)
            ticket_booking.Booking_Expiration_Time = null;
            //Оновлюємо статус квитка
            ticket_booking.Ticket_Status = TicketStatus.Booked_And_Active;
            //Оновлюємо в базі інформацію про квиток
            await full_ticket_management_service.UpdateTicketBooking(ticket_booking);
            ExternalOutputCompletedTicketBookingDto finished_ticket_booking_dto = new ExternalOutputCompletedTicketBookingDto
            {
                Id = ticket_booking.Id,
                User_Id = ticket_booking.User_Id,
                Train_Route_On_Date_Id = ticket_booking.Train_Route_On_Date_Id,
                Passenger_Carriage_Id = ticket_booking.Passenger_Carriage_Id,
                Passenger_Carriage_Position_In_Squad = ticket_booking.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = ticket_booking.Place_In_Carriage,
                Booking_Completion_Time = ticket_booking.Booking_Time,
                Starting_Station_Title = input_unfinished_ticket.Starting_Station_Title,
                Ending_Station_Title = input_unfinished_ticket.Ending_Station_Title,
                Passenger_Name = ticket_booking.Passenger_Name,
                Passenger_Surname = ticket_booking.Passenger_Surname,
                Ticket_Status = TextEnumConvertationService.GetTicketBookingStatusIntoString(ticket_booking.Ticket_Status),
            };
            return new SuccessQuery<ExternalOutputCompletedTicketBookingDto>(finished_ticket_booking_dto);
        }
        public async Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> CancelTicketBookingReservationForUser(ExternalOutputMediatorTicketBookingDto input_unfinished_ticket)
        {
            //Отримуємо аутентифікованого користувача
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            //Отримуємо з бази запис про поточне бронювання
            TicketBooking? ticket_booking = await full_ticket_management_service.FindTicketBookingById(input_unfinished_ticket.Id);
            //Перевірка, чи квиток є в базі
            if (ticket_booking is null)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.InternalServerError, "System can't find the ticket to complete booking"));
            }
            //Перевірка, чи аутентифікований користувач є тим самим, який розпочав процес бронювання квитка
            if (user.Id != input_unfinished_ticket.User_Id)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.Forbidden, "Authenticated user doesn't own the ticket whose booking is being tried to be completed"));
            }
            //Перевірка,чи не сплив час бронювання
            if (DateTime.Now > ticket_booking.Booking_Expiration_Time)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.Forbidden, "Booking reservation time has expired"));
            }
            if (ticket_booking.Ticket_Status != TicketStatus.Booking_In_Progress)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.BadRequest, "This place has already been booked"));
            }
            await full_ticket_management_service.DeleteTicketBooking(ticket_booking);
            return new SuccessQuery<ExternalOutputMediatorTicketBookingDto>(input_unfinished_ticket);

        }
        

        public async Task DeleteAllExpiredBookings()
        {
            await full_ticket_management_service.DeleteAllExpiredTickets();
        }
    }
}
 
      
    /*
     public async QueryResult<MediatorTicketBookingDto> InitialzeTicketBookingProcess(InitialTicketBookingDto input)
     {
         QueryResult<User> user_authentication_result = await user_management_service.GetAuthenticatedUser();
         if (user_authentication_result.Fail)
         {
             return new FailQuery<MediatorTicketBookingDto>(user_authentication_result.Error);
         }
         User authenticated_user = user_authentication_result.Value;

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
     }*/
    /*
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
               Ticket_Status =  GetTicketBookingStatusIntoString(unfinished_ticket_booking.Ticket_Status),
               Train_Route_On_Date_Id = unfinished_ticket_booking.Train_Route_On_Date_Id,
               Passenger_Name = unfinished_ticket_booking.Passenger_Name,
               Passenger_Surname = unfinished_ticket_booking.Passenger_Surname
           };
       }
       */
        /*
        public async Task<QueryResult<ExternalOutputCompletedTicketBookingDto>> CompleteTicketBookingProcessForAuthenticatedUser_OLDVERSION
            (ExternalOutputMediatorTicketBookingDto input_unfinished_ticket, ExternalInputPassengerInfoForCompletedTicketBookingDto passenger_info)
        {
            //Отримуємо аутентифікованого користувача
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            //Отримуємо з бази запис про поточне бронювання
            TicketBooking? ticket_booking = await full_ticket_management_service.FindTicketBookingById(input_unfinished_ticket.Id);
            //Перевірка, чи квиток є в базі
            if (ticket_booking is null)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.InternalServerError, "System can't find the ticket to complete booking"));
            }
            //Перевірка, чи аутентифікований користувач є тим самим, який розпочав процес бронювання квитка
            if (user.Id != input_unfinished_ticket.User_Id)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.Forbidden, "Authenticated user doesn't own the ticket whose booking is being tried to be completed"));
            }
            //Перевірка,чи не сплив час бронювання
            if (DateTime.Now > ticket_booking.Booking_Expiration_Time)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.Forbidden, "Booking reservation time has expired"));
            }
            //Встановлюємо інформацію про пасажира
            ticket_booking.Passenger_Name = passenger_info.Passenger_Name;
            ticket_booking.Passenger_Surname = passenger_info.Passenger_Surname;
            //Оновлюємо час бронювання
            ticket_booking.Booking_Time = DateTime.Now;
            //Видаляємо інформацію про час закінчення періоду бронювання(адже бронювання завершено)
            ticket_booking.Booking_Expiration_Time = null;
            //Оновлюємо статус квитка
            ticket_booking.Ticket_Status = TicketStatus.Booked_And_Active;
            //Оновлюємо в базі інформацію про квиток
            await full_ticket_management_service.UpdateTicketBooking(ticket_booking);
            ExternalOutputCompletedTicketBookingDto finished_ticket_booking_dto = new ExternalOutputCompletedTicketBookingDto
            {
                Id = ticket_booking.Id,
                User_Id = ticket_booking.User_Id,
                Train_Route_On_Date_Id = ticket_booking.Train_Route_On_Date_Id,
                Passenger_Carriage_Id = ticket_booking.Passenger_Carriage_Id,
                Passenger_Carriage_Position_In_Squad = ticket_booking.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = ticket_booking.Place_In_Carriage,
                Booking_Completion_Time = ticket_booking.Booking_Time,
                Starting_Station_Title = input_unfinished_ticket.Starting_Station_Title,
                Ending_Station_Title = input_unfinished_ticket.Ending_Station_Title,
                Passenger_Name = ticket_booking.Passenger_Name,
                Passenger_Surname = ticket_booking.Passenger_Surname,
                Ticket_Status = TextEnumConvertationService.GetTicketBookingStatusIntoString(ticket_booking.Ticket_Status),
            };
            return new SuccessQuery<ExternalOutputCompletedTicketBookingDto>(finished_ticket_booking_dto);
        }
    }
        */