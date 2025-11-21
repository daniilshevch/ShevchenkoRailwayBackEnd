using Microsoft.OpenApi.Writers;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
namespace RailwayManagementSystemAPI.ExternalServices.SystemServices
{
    public class ExpiredTicketBookingsRemovingService: BackgroundService
    {
        private readonly IServiceScopeFactory scope_factory;
        private const int clean_up_interval = 1;
        public ExpiredTicketBookingsRemovingService(IServiceScopeFactory scope_factory)
        {
            this.scope_factory = scope_factory;
        }
        protected override async Task ExecuteAsync(CancellationToken stopping_token)
        {
            while(!stopping_token.IsCancellationRequested)
            {
                try
                {
                    using IServiceScope scope = scope_factory.CreateScope();
                    IServiceProvider service_provider = scope.ServiceProvider;
                    ICompleteTicketBookingProcessingService ticket_booking_service = service_provider.GetRequiredService<ICompleteTicketBookingProcessingService>();
                    await ticket_booking_service.DeleteAllExpiredBookings();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await Task.Delay(TimeSpan.FromMinutes(clean_up_interval), stopping_token);
            }
        }
    }
}
