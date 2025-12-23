using RailwayCore.Models.ModelEnums.TicketBookingEnums;
using RailwayCore.Models.ModelEnums.TrainRouteEnums;
using RailwayCore.Models.ModelEnums.PassengerCarriageEnums;
using RailwayCore.Models.ModelEnums.UserEnums;
namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices
{
    public class TextEnumConvertationService
    {
        [Checked("19.04.2025")]
        public static string GetCarriageTypeIntoString(PassengerCarriageType carriage_type)
        {
            switch (carriage_type)
            {
                case PassengerCarriageType.Platskart:
                    return "Platskart";
                case PassengerCarriageType.Coupe:
                    return "Coupe";
                case PassengerCarriageType.SV:
                    return "SV";
                case PassengerCarriageType.Sitting:
                    return "Sitting";
                default:
                    return "";
            }
        }
        [Checked("19.04.2025")]
        public static string? GetCarriageQualityClassIntoString(PassengerCarriageQualityClass? quality_class)
        {
            if (quality_class == null)
            {
                return null;
            }
            switch (quality_class)
            {
                case PassengerCarriageQualityClass.S:
                    return "S";
                case PassengerCarriageQualityClass.A:
                    return "A";
                case PassengerCarriageQualityClass.B:
                    return "B";
                case PassengerCarriageQualityClass.C:
                    return "C";
                default:
                    return "";
            }
        }
        [Checked("19.04.2025")]
        public static string? GetTrainQualityClassIntoString(TrainQualityClass? quality_class)
        {
            if (quality_class == null)
            {
                return null;
            }
            switch (quality_class)
            {
                case TrainQualityClass.S:
                    return "S";
                case TrainQualityClass.A:
                    return "A";
                case TrainQualityClass.B:
                    return "B";
                case TrainQualityClass.C:
                    return "C";
                default:
                    return "";
            }
        }
        public static string? GetUserSexIntoString(Sex? sex)
        {
            if(sex is null)
            {
                return null;
            }
            switch (sex)
            {
                case Sex.Male:
                    return "Male";
                case Sex.Female:
                    return "Female";
                default:
                    return null;
            }
        }
        public static Sex? GetUserSexIntoEnum(string? sex)
        {
            if(sex is null)
            {
                return null;
            }
            switch(sex)
            {
                case "Male":
                    return Sex.Male;
                case "Female":
                    return Sex.Female;
                default:
                    return null;
            }
        }
        public static string GetUserRoleIntoString(Role role)
        {
            switch (role)
            {
                case Role.Administrator:
                    return "Administrator";
                case Role.General_User:
                    return "General User";
                default:
                    return "General User";
            }
        }
        public static string GetTicketBookingStatusIntoString(TicketStatus ticket_status)
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
                case TicketStatus.Archived:
                    return "Archieved";
                case TicketStatus.Booking_Failed:
                    return "Booking_Failed";
                default:
                    return "";
            }
        }
    }
}
