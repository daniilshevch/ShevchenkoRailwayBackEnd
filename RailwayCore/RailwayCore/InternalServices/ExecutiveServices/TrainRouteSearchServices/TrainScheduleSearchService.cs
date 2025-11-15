using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using RailwayCore.InternalServices.ModelRepositories.Implementations;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices
{
    /// <summary>
    /// Даний сервіс дозволяє виконувати запити для пошуку інформації, пов'язаної з розкладом руху поїздів
    /// </summary>
    [ExecutiveService]
    public class TrainScheduleSearchService
    {
        private readonly AppDbContext context;
        private readonly StationRepository station_repository;
        private readonly TrainRouteOnDateRepository train_route_on_date_repository;
        public TrainScheduleSearchService(AppDbContext context, StationRepository station_repository, TrainRouteOnDateRepository train_route_on_date_repository)
        {
            this.context = context;
            this.station_repository = station_repository;
            this.train_route_on_date_repository = train_route_on_date_repository;

        }
        /// <summary>
        /// Вертає список зупинок поїзда(якщо order_mode = true(за замовчування так і є), то список посортований)
        /// </summary>
        /// <param name="train_route_on_date_id"></param>
        /// <param name="order_mode"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id, bool order_mode = true)
        {
            //Знаходимо рейс поїзда в дату за айді
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Train_Stops)
                .ThenInclude(train_stop => train_stop.Station)
                .Include(train_route_on_date => train_route_on_date.Train_Route)
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return null;
            }
            //Отримуємо зупинки поїзда 
            List<TrainRouteOnDateOnStation> train_stops = train_route_on_date.Train_Stops;
            if (order_mode)
            {
                train_stops = train_stops.OrderBy(train_stop => train_stop.Arrival_Time).ToList();
            }
            return train_stops;
        }
        /// <summary>
        /// Вертає список зупинок поїзда(якщо order_mode = true(за замовчування так і є), то список посортований)
        /// </summary>
        /// <param name="train_route_id"></param>
        /// <param name="departure_date"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            string train_route_on_date_id = train_route_on_date_repository.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            return await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
        }

        /// <summary>
        /// Вертає список зупинок для декількох поїздів, причому сортує цей список спочатку за рейсом поїзда, а потім за порядком, в якому їх 
        /// проходить цей самий рейс(попідряд стоять зупинки одного рейсу поїзда, посортовані в порядку слідування)
        /// </summary>
        /// <param name="train_route_on_date_ids"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<List<TrainRouteOnDateOnStation>> GetTrainStopsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            //Отримуємо зупинки для групи рейсів поїздів в дату, посортовані спочатку за номером рейсу поїзда
            //(попідряд будуть стояти зупинки одного поїзда), а також посортовані за часом прибуття поїзда на станцію
            List<TrainRouteOnDateOnStation> train_stops_for_several_train_routes_on_date = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Include(train_stop => train_stop.Station)
                .Where(train_stop => train_route_on_date_ids.Contains(train_stop.Train_Route_On_Date_Id))
                .OrderBy(train_stop => train_stop.Train_Route_On_Date_Id).ThenBy(train_stop => train_stop.Arrival_Time).ToListAsync();
            return train_stops_for_several_train_routes_on_date;
        }
        /// <summary>
        /// Вертає список поїзда між двома зупинками, переданими в параметри функції, включно з ними самими
        /// </summary>
        /// <param name="train_route_on_date_id"></param>
        /// <param name="first_station_title"></param>
        /// <param name="second_station_title"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(string train_route_on_date_id, string first_station_title, string second_station_title)
        {
            //Отримуємо всі зупинки рейса поїзда в дату(посортовані)
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (train_stops == null)
            {
                return null;
            }
            //Отримуємо початкову та кінцеву станції, між якими ми хочемо знайти всі зупинки
            Station? starting_station = await station_repository.GetStationByTitle(first_station_title);
            Station? ending_station = await station_repository.GetStationByTitle(second_station_title);
            if (starting_station is null || ending_station is null)
            {
                return null;
            }
            //Отримуємо номер в списку зупинок при слідування поїзда, який мають початоква та кінцева бажані зупинки
            int starting_station_number = train_stops.FindIndex(train_route => train_route.Station.Title == first_station_title);
            int ending_station_number = train_stops.FindIndex(train_route => train_route.Station.Title == second_station_title);
            if (starting_station_number == -1 || ending_station_number == -1)
            {
                return null;
            }
            List<TrainRouteOnDateOnStation> final_train_stops = new List<TrainRouteOnDateOnStation>();
            //Перебираємо всі зупинки між бажаною початковою та кінцевою та додаємо їх в масив
            for(int station_number = starting_station_number; station_number <= ending_station_number; station_number++)
            {
                final_train_stops.Add(train_stops[station_number]);
            }
            return final_train_stops;

        }

        /// <summary>
        /// Вертає початкову зупинку рейсу поїзда
        /// </summary>
        /// <param name="train_route_on_date_id"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<TrainRouteOnDateOnStation?> GetStartingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            //Вертаємо початкову зупинку рейсу поїзда
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode: true);
            if (train_stops == null)
            {
                return null;
            }
            return train_stops[0];
        }
        /// <summary>
        /// Вертає кінцеву зупинку рейсу поїзда
        /// </summary>
        /// <param name="train_route_on_date_id"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<TrainRouteOnDateOnStation?> GetEndingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            //Вертаємо кінцеву зупинку рейсу поїзда
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode: true);
            if (train_stops == null)
            {
                return null;
            }
            return train_stops[train_stops.Count - 1];
        }
        /// <summary>
        /// Вертає об'єкт зупинки за айді рейсу та айді станції
        /// </summary>
        /// <param name="train_route_on_date_id"></param>
        /// <param name="station_id"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<TrainRouteOnDateOnStation?> GetTrainStopInfoByTrainRouteOnDateIdAndStationId(string train_route_on_date_id, int station_id)
        {
            //Отримуємо об'єкт зупинки за айді рейсу поїзда та айді станції
            return await context.Train_Routes_On_Date_On_Stations
                .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id &&
                train_stop.Station_Id == station_id);
        }
    }
}
