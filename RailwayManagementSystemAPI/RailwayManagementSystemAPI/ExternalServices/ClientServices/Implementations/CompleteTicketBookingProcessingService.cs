using RailwayCore.Models;
using RailwayCore.InternalServices.ExecutiveServices.ExecutiveDTO.TicketManagementDTO;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;
using System.Diagnostics;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
using RailwayCore.InternalServices.CoreServices.Implementations;
using RailwayCore.InternalServices.CoreServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    /// <summary>
    /// Даний сервіс займається всіми задачами, які пов'язані з процесом бронювання та купівлі квитків користувачами. Цей сервіс обробляє процес
    /// первинної тимчасової броні, коли користувач обирає квитки, кінцевої броні з записом квитка на певне ім'я, а також займається скасування
    /// первинної броні для квитків, коли місце тимчасово резерувується для користувача, але він ще його не купив 
    /// - при чому поверненням вже придбаних квитків займається сервіс UserTicketManagementService. 
    /// Також цей сервіс не займається пошуком бронювань(під час пошуку поїздів з вільними місцями - цим 
    /// займається TrainRouteWithBookingsSearchService, а тільки реалізує нові броні для користувача. Крім того, цей сервіс не займається 
    /// пошуком вже існуючих бронювань для користувача - цим займається UserTicketManagementService, поверненням вже придбаних квитків теж він.
    /// </summary>
    [ClientApiService]
    public class CompleteTicketBookingProcessingService : ICompleteTicketBookingProcessingService
    {
        private readonly string service_name = "CompleteTicketBookingProcessingService";
        private readonly IFullTicketManagementService full_ticket_management_service;
        private readonly SystemAuthenticationService system_authentication_service;
        private readonly IConfiguration configuration;
        public CompleteTicketBookingProcessingService(IFullTicketManagementService full_ticket_management_service, SystemAuthenticationService system_authentication_service, IConfiguration configuration)
        {
            this.full_ticket_management_service = full_ticket_management_service;
            this.system_authentication_service = system_authentication_service;
            this.configuration = configuration;
        }
        /// <summary>
        /// Даний метод має спрацьовувати, коли користувач обирає місце в вагоні в певному рейсі і починає процес заповнення інформації про себе та поїздку.
        /// Фактично, він розпочинає процес бронювання і резервує дане місце за даним користувачем на певний час. При цьому прізвище, ім'я та конкретна інфомрація про 
        /// пасажира та поїздку не вводяться, але місце тимчасово позначене, як зайняте.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcessForUser
    (ExternalInputInitialTicketBookingDto input, User user)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------BOOKING INITIALIZATION PROCESS------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
            //Робимо ініціалізацію квитка, виставляючи всі дані бажаної подорожі і прив'язуючи квиток до акаунту, але не вказуємо ім'я пасажира
            //Статус квитка - Booking_In_Progress
            InternalTicketBookingWithCarriagePositionDto internal_ticket_dto = new InternalTicketBookingWithCarriagePositionDto
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

            //Видаємо проміжну інформацію про квиток на даний момент(бронювання вже ініціалізоване і перебуває в процесі)
            ExternalOutputMediatorTicketBookingDto mediator_ticket_booking_dto = new ExternalOutputMediatorTicketBookingDto
            {
                Id = current_ticket_booking.Id,
                Full_Ticket_Id = current_ticket_booking.Full_Ticket_Id.ToString()!,
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
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Booking time: ");
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            Console.ResetColor();
            return new SuccessQuery<ExternalOutputMediatorTicketBookingDto>(mediator_ticket_booking_dto, new SuccessMessage($"Booking reservation successfully " +
                $"initialized\nBooking Id: {current_ticket_booking.Full_Ticket_Id}\nBooking expiration time: {mediator_ticket_booking_dto.Booking_Expiration_Time}", annotation: service_name, unit: ProgramUnit.ClientAPI));

        }
        /// <summary>
        /// Ініціалізовує процес бронювання квитка для користувача, який аутентифікований.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ClientApiMethod]
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
        /// <summary>
        /// Проводить ініціалізацію бронювання одразу декількох квитків
        /// </summary>
        /// <param name="ticket_bookings_list"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<List<ExternalOutputMediatorTicketBookingDto>>> InitializeMultipleTicketBookingProcessForAuthenticatedUser(
            List<ExternalInputInitialTicketBookingDto> ticket_bookings_list)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------MULTIPLE BOOKING INITIALIZATION PROCESS------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
            //Отримуємо аутентифікованого користувача
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalOutputMediatorTicketBookingDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            List<ExternalOutputMediatorTicketBookingDto> ticket_bookings = new List<ExternalOutputMediatorTicketBookingDto>();
            //Проводимо процес бронювання декількох квитків
            foreach (ExternalInputInitialTicketBookingDto ticket_booking_dto in ticket_bookings_list)
            {
                //Пробуємо ініціалізувати бронювання кожного окремо взятого квитка
                QueryResult<ExternalOutputMediatorTicketBookingDto> ticket_booking_result =
                    await InitializeTicketBookingProcessForUser(ticket_booking_dto, user);
                if (ticket_booking_result.Fail)
                {
                    //Якщо для певного квитка не вдалась ініціалізація броні, не вертаємо FailQuery, як в випадку одного квитка, а вертаємо цей квиток зі статусом
                    //Booking Failed.
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
                else //Якщо бронь пройшла успішно, вертаємо квиток зі статусом Booking_In_Progress
                {
                    ExternalOutputMediatorTicketBookingDto successfully_booked_ticket = ticket_booking_result.Value;
                    ticket_bookings.Add(successfully_booked_ticket);
                }
            }
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Total Booking time: ");
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            Console.ResetColor();
            return new SuccessQuery<List<ExternalOutputMediatorTicketBookingDto>>(ticket_bookings, new SuccessMessage("Multiple ticket booking reservation attempt" +
                " has been performed. Results may be observed above", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }

        /// <summary>
        /// Даний метод завершує процес бронювання квитка для користувача. Коли бронь вже була ініціалізована і користувач заповнює інформацію про пасажира та 
        /// додаткову інформацію про поїздку.
        /// </summary>
        /// <param name="input_unfinished_ticket"></param>
        /// <param name="passenger_info"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<ExternalOutputCompletedTicketBookingDto>> CompleteTicketBookingProcessForAuthenticatedUser
            (ExternalOutputMediatorTicketBookingDto input_unfinished_ticket, ExternalInputPassengerInfoForCompletedTicketBookingDto passenger_info)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------BOOKING COMPLETION PROCESS------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
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
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.InternalServerError, "System can't find the ticket to complete booking. Probably, ticket expiration time came down",
                    annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Перевірка, чи аутентифікований користувач є тим самим, який розпочав процес бронювання квитка
            if (user.Id != input_unfinished_ticket.User_Id)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.Forbidden, "Authenticated user doesn't own the ticket whose booking is being tried to be completed",
                    annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Перевірка,чи не сплив час бронювання
            if (DateTime.Now > ticket_booking.Booking_Expiration_Time)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.Forbidden, "Booking reservation time has expired", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            if (ticket_booking.Ticket_Status != TicketStatus.Booking_In_Progress)
            {
                return new FailQuery<ExternalOutputCompletedTicketBookingDto>(new Error(ErrorType.BadRequest, "This place has already been booked", annotation: service_name, unit: ProgramUnit.ClientAPI));
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
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Booking time: ");
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            Console.ResetColor();
            return new SuccessQuery<ExternalOutputCompletedTicketBookingDto>(finished_ticket_booking_dto, new SuccessMessage($"Booking reservation for ticket with Id: " +
                $"{ticket_booking.Full_Ticket_Id} has been successfully completed", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }

        /// <summary>
        /// Даний метод проводить скасування тимчасової резервації квитків для користувача під час його їх потенційного придбання.
        /// Тобто якщо користувач обрав місце і почав його оформлення, але потім вирішив припинити це саме оформлення, то цей метод
        /// скасує тимчасову резервацію місця. Важливо: повернення вже придбаних квитків - це задача іншого сервісу - UserTicketManagementService.
        /// </summary>
        /// <param name="input_unfinished_ticket"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> CancelTicketBookingReservationForUser(ExternalOutputMediatorTicketBookingDto input_unfinished_ticket)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------TEMPORARY TICKET RESERVATION CANCELLATION PROCESS------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
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
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.InternalServerError, "System can't find the ticket to complete booking",
                    annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Перевірка, чи аутентифікований користувач є тим самим, який розпочав процес бронювання квитка
            if (user.Id != input_unfinished_ticket.User_Id)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.Forbidden, "Authenticated user doesn't own the ticket whose booking is being tried to be completed",
                    annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Перевірка,чи не сплив час бронювання
            if (DateTime.Now > ticket_booking.Booking_Expiration_Time)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.Forbidden, "Booking reservation time has expired",
                    annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            if (ticket_booking.Ticket_Status != TicketStatus.Booking_In_Progress)
            {
                return new FailQuery<ExternalOutputMediatorTicketBookingDto>(new Error(ErrorType.BadRequest, "This place has already been booked",
                    annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            await full_ticket_management_service.DeleteTicketBooking(ticket_booking);
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Temporary reservation cancellation time: ");
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            Console.ResetColor();
            return new SuccessQuery<ExternalOutputMediatorTicketBookingDto>(input_unfinished_ticket,
                new SuccessMessage($"Temporary ticket reservation was cancelled for ticket {input_unfinished_ticket.Full_Ticket_Id}",
                annotation: service_name, unit: ProgramUnit.ClientAPI));

        }

        public async Task DeleteAllExpiredBookings()
        {
            await full_ticket_management_service.DeleteAllExpiredTickets();
        }
    }
}
 
      
   