using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models.ModelEnums.TicketBookingEnums
{
    public enum TicketStatus
    {
        Booking_In_Progress,
        Booked_And_Active,
        Booked_And_Used,
        Archieved,
        Returned,
        Booking_Failed
    }
}
