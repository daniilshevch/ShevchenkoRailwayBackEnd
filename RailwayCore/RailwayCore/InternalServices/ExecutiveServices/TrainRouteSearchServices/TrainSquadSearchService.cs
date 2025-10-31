using RailwayCore.Context;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices
{
    [ExecutiveService]
    public class TrainSquadSearchService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateRepository train_route_on_date_service;
        public TrainSquadSearchService(AppDbContext context, TrainRouteOnDateRepository train_route_on_date_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
        }
        [Refactored("v1", "18.04.2025")]
        [Checked("04.07.2025")]
        [Executive]
        [ExecutiveMethod]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_on_date_id)
        {
            //Отримуємо рейс поїзда з інформацією про вагони
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Carriage_Assignements)
                .ThenInclude(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return null;
            }
            //Сортуємо вагони за номером в складі і вертаємо користувачу
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments = train_route_on_date.Carriage_Assignements
                .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();
            return carriage_assignments;
        }

        [Checked("18.04.2025")]
        [Checked("04.07.2025")]
        [ExecutiveService]
        [Peripheral]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            string train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            return await GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id);
        }

        [Refactored("v1", "18.04.2025")]
        [Checked("04.07.2025")]
        [Crucial]
        [ExecutiveService]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            //Отримуємо вагони в складі декількох рейсів поїздів в певні дати разом, 
            //сортуємо спочатку за номером рейсу(всі вагони одного поїзда будуть в списку попідряд), а потім за номером вагона в складі
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignment => carriage_assignment.Train_Route_On_Date)
                .Include(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .Where(carriage_assignment => train_route_on_date_ids.Contains(carriage_assignment.Train_Route_On_Date_Id))
                .OrderBy(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id)
                .ThenBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToListAsync();
            return carriage_assignments;
        }
    }
}
