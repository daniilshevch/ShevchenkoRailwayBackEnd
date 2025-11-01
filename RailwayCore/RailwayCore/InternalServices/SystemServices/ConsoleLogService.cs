using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RailwayCore.InternalServices.SystemServices
{
    public static class ConsoleLogService
    {
        public static void PrintUnit(ProgramUnit? unit)
        {
            if(unit == null)
            {
                return;
            }
            switch(unit)
            {
                case ProgramUnit.Core:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[Core]");
                    Console.ResetColor();
                    return;
                case ProgramUnit.ClientAPI:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[Client API]");
                    Console.ResetColor();
                    return;
                case ProgramUnit.AdminAPI:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[Admin API]");
                    Console.ResetColor();
                    return;
                default:
                    return;

            }
        }
        public static void PrintAnnotationForFailQuery(string? annotation)
        {
            if (annotation != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{annotation}] ");
                Console.ResetColor();
            }
        }
        public static void PrintAnnotationForSuccessQuery(string? annotation)
        {
            if (annotation != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{annotation}] ");
                Console.ResetColor();
            }
        }
        public static void PrintMessage(string? message)
        {
            if (message != string.Empty)
            {
                Console.WriteLine(message);
            }
        }
        public static string PrintRaces(List<string> train_races)
        {
            string result = string.Empty;
            result += "[";
            for(int race_index = 0; race_index < train_races.Count; race_index++)
            {
                result += $"{train_races[race_index]}";
                if(race_index != train_races.Count - 1)
                {
                    result += ", ";
                }
            }
            result += "]";
            return result;
        }
        public static string PrintTicketBooking(TicketBooking ticket_booking)
        {
            string result = string.Empty;
            result += $"Ticket ID: {ticket_booking.Full_Ticket_Id}\n";
            result += $"Train Race: {ticket_booking.Train_Route_On_Date_Id}\n";
            result += ticket_booking.Starting_Station is not null ? $"Starting Station: {ticket_booking.Starting_Station.Title}\n" : $"Starting Station: {ticket_booking.Starting_Station_Id}\n";
            result += ticket_booking.Ending_Station is not null ? $"Ending Station: {ticket_booking.Ending_Station.Title}\n": $"Ending Station: {ticket_booking.Ending_Station_Id}\n";
            result += $"Carriage Id: {ticket_booking.Passenger_Carriage_Id}\n";
            result += $"Carriage Position: {ticket_booking.Passenger_Carriage_Position_In_Squad}\n";
            result += $"Place In Carriage: {ticket_booking.Place_In_Carriage}\n";
            result += $"Passenger Full Name: {ticket_booking.Passenger_Name}  {ticket_booking.Passenger_Surname}\n";
            result += $"Booking Expiration Time: {ticket_booking.Booking_Expiration_Time}\n";
            result += $"Status: {ticket_booking.Ticket_Status}";
            return result;
        }
    }
}
