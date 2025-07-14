using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ExecutiveServices
{
    public class TicketAdditionalServicesManager
    {
        public string StringifyTicketAdditionalServices(InternalTicketAdditionalServicesDto additional_services)
        {
            return JsonSerializer.Serialize(additional_services);
        }
    }
}
