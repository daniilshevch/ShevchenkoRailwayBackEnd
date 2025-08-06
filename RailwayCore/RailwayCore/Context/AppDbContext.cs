using Microsoft.EntityFrameworkCore;
using RailwayCore.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RailwayCore.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<RailwayBranch> Railway_Branches { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<PassengerCarriage> Passenger_Carriages { get; set; }
        public DbSet<TrainRoute> Train_Routes { get; set; }
        public DbSet<TrainRouteOnDate> Train_Routes_On_Date { get; set; }
        public DbSet<TrainRouteOnDateOnStation> Train_Routes_On_Date_On_Stations { get; set; }
        public DbSet<PassengerCarriageOnTrainRouteOnDate> Passenger_Carriages_On_Train_Routes_On_Date { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TicketBooking> Ticket_Bookings { get; set; }
        public DbSet<TicketInSelling> Tickets_In_Selling { get; set; }
        public DbSet<UserProfile> User_Profiles { get; set; }
        public DbSet<Image> Images { get; set; }
        public AppDbContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options_builder)
        {
            options_builder.UseMySql("User = root; Database = RailwayManagementSystem; Server = localhost; Password = Mansion20051505;",
                new MySqlServerVersion(new Version(8, 0, 33)));
        }
        protected override void OnModelCreating(ModelBuilder model_builder)
        {
            //1. Station
            model_builder.Entity<Station>()
                .HasOne(station => station.Railway_Branch)
                .WithMany(railway_branch => railway_branch.Stations)
                .HasForeignKey(station => station.Railway_Branch_Id)
                .HasConstraintName("Station@Railway_Branch$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<Station>()
                .Property(station => station.Type_Of)
                .HasConversion<string>();
            model_builder.Entity<Station>()
                .Property(station => station.Region)
                .HasConversion<string>();
            model_builder.Entity<Station>().ToTable("Station");
            model_builder.Entity<Station>()
                .HasIndex(station => station.Title)
                .HasDatabaseName("Station%Title$IDX")
                .IsUnique(true);

            //2. Passenger_Carriage
            model_builder.Entity<PassengerCarriage>()
                .HasOne(carriage => carriage.Station_Depot)
                .WithMany(station => station.Carriages_In_Depot)
                .HasForeignKey(carriage => carriage.Station_Depot_Id)
                .HasConstraintName("Passenger_Carriage@Station_Depot$FK")
                .OnDelete(DeleteBehavior.SetNull);
            model_builder.Entity<PassengerCarriage>()
                .Property(carriage => carriage.Type_Of)
                .HasConversion<string>();
            model_builder.Entity<PassengerCarriage>()
                .Property(carriage => carriage.Manufacturer)
                .HasConversion<string>();
            model_builder.Entity<PassengerCarriage>()
                .Property(carriage => carriage.Quality_Class)
                .HasConversion<string>();
            model_builder.Entity<PassengerCarriage>().ToTable("Passenger_Carriage");

            //3. Train_Route
            model_builder.Entity<TrainRoute>()
                .HasOne(train_route => train_route.Railway_Branch)
                .WithMany(railway_branch => railway_branch.Train_Routes)
                .HasForeignKey(train_route => train_route.Railway_Branch_Id)
                .HasConstraintName("Train_Route@Railway_Branch$FK")
                .OnDelete(DeleteBehavior.SetNull);
            model_builder.Entity<TrainRoute>()
                .Property(train_route => train_route.Speed_Type)
                .HasConversion<string>();
            model_builder.Entity<TrainRoute>()
                .Property(train_route => train_route.Frequency_Type)
                .HasConversion<string>();
            model_builder.Entity<TrainRoute>()
                .Property(train_route => train_route.Assignement_Type)
                .HasConversion<string>();
            model_builder.Entity<TrainRoute>()
                .Property(train_route => train_route.Quality_Class)
                .HasConversion<string>();
            model_builder.Entity<TrainRoute>()
                .Property(train_route => train_route.Trip_Type)
                .HasConversion<string>();
            model_builder.Entity<TrainRoute>().ToTable("Train_Route");

            //4. Train_Route_On_Date
            model_builder.Entity<TrainRouteOnDate>()
                .HasOne(train_route_on_date => train_route_on_date.Train_Route)
                .WithMany(train_route => train_route.Train_Assignements)
                .HasForeignKey(train_route_on_date => train_route_on_date.Train_Route_Id)
                .HasConstraintName("Train_Route_On_Date@Train_Route$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TrainRouteOnDate>().ToTable("Train_Route_On_Date");

            //5. Train_Route_On_Date_On_Station
            model_builder.Entity<TrainRouteOnDateOnStation>()
                .HasKey(train_route_on_date_on_station => new
                {
                    train_route_on_date_on_station.Train_Route_On_Date_Id,
                    train_route_on_date_on_station.Station_Id
                });
            //Many with Many connection 
            model_builder.Entity<TrainRouteOnDate>()
                .HasMany(train_route_on_date => train_route_on_date.Stations)
                .WithMany(station => station.Train_Routes_On_Date)
                .UsingEntity<TrainRouteOnDateOnStation>
                (train_route_on_date_on_station => train_route_on_date_on_station
                .HasOne(train_route_on_date_on_station => train_route_on_date_on_station.Station)
                .WithMany(station => station.Train_Stops)
                .HasForeignKey(train_route_on_date_on_station => train_route_on_date_on_station.Station_Id)
                .HasConstraintName("Train_Route_On_Date_On_Station@Station$FK")
                .OnDelete(DeleteBehavior.Cascade),
                train_route_on_date_on_station => train_route_on_date_on_station
                .HasOne(train_route_on_date_on_station => train_route_on_date_on_station.Train_Route_On_Date)
                .WithMany(train_route_on_date => train_route_on_date.Train_Stops)
                .HasForeignKey(train_route_on_date_on_station => train_route_on_date_on_station.Train_Route_On_Date_Id)
                .HasConstraintName("Train_Route_On_Date_On_Station@Train_Route_On_Date$FK")
                .OnDelete(DeleteBehavior.Cascade));
            model_builder.Entity<TrainRouteOnDateOnStation>()
                .Property(train_route_on_date_on_station => train_route_on_date_on_station.Arrival_Time)
                .HasColumnType("DATETIME(0)");
            model_builder.Entity<TrainRouteOnDateOnStation>()
               .Property(train_route_on_date_on_station => train_route_on_date_on_station.Departure_Time)
               .HasColumnType("DATETIME(0)");
            model_builder.Entity<TrainRouteOnDateOnStation>()
                .Property(train_route_on_date_on_starion => train_route_on_date_on_starion.Stop_Type)
                .HasConversion<string>();
            model_builder.Entity<TrainRouteOnDateOnStation>().ToTable("Train_Route_On_Date_On_Station");

            //6. Passenger_Carriage_On_Train_Route_On_Date
            model_builder.Entity<PassengerCarriageOnTrainRouteOnDate>()
                .HasKey(passenger_carriage_on_train_route_on_date => new
                {
                    passenger_carriage_on_train_route_on_date.Passenger_Carriage_Id,
                    passenger_carriage_on_train_route_on_date.Train_Route_On_Date_Id
                });
            //Many with Many connection

            model_builder.Entity<PassengerCarriage>()
                .HasMany(carriage => carriage.Train_Routes_On_Date)
                .WithMany(train_route_on_date => train_route_on_date.Passenger_Carriages)
                .UsingEntity<PassengerCarriageOnTrainRouteOnDate>
                (
                passenger_carriage_on_train_route_on_date => passenger_carriage_on_train_route_on_date
                .HasOne(passenger_carriage_on_train_route_on_date => passenger_carriage_on_train_route_on_date.Train_Route_On_Date)
                .WithMany(train_route_on_date => train_route_on_date.Carriage_Assignements)
                .HasForeignKey(passenger_carriage_on_train_route_on_date => passenger_carriage_on_train_route_on_date.Train_Route_On_Date_Id)
                .HasConstraintName("Passenger_Carriage_On_Train_Route_On_Date@Train_Route_On_Date$FK")
                .OnDelete(DeleteBehavior.Cascade),
                 passenger_carriage_on_train_route_on_date => passenger_carriage_on_train_route_on_date
                .HasOne(passenger_carriage_on_train_route_on_date => passenger_carriage_on_train_route_on_date.Passenger_Carriage)
                .WithMany(carriage => carriage.Carriage_Assignements)
                .HasForeignKey(passenger_carriage_on_train_route_on_date => passenger_carriage_on_train_route_on_date.Passenger_Carriage_Id)
                .HasConstraintName("Passenger_Carriage_On_Train_Route_On_Date@Passenger_Carriage$FK")
                .OnDelete(DeleteBehavior.Cascade));
            model_builder.Entity<PassengerCarriageOnTrainRouteOnDate>().ToTable("Passenger_Carriage_On_Train_Route_On_Date");

            //7. Ticket_Booking
            model_builder.Entity<TicketBooking>()
                .HasOne(ticket => ticket.Train_Route_On_Date)
                .WithMany(train_route_on_date => train_route_on_date.Ticket_Bookings)
                .HasForeignKey(ticket => ticket.Train_Route_On_Date_Id)
                .HasConstraintName("Ticket_Booking@Train_Route_On_Date$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketBooking>()
                .HasOne(ticket => ticket.Passenger_Carriage)
                .WithMany(carriage => carriage.Ticket_Bookings)
                .HasForeignKey(ticket => ticket.Passenger_Carriage_Id)
                .HasConstraintName("Ticket_Booking@Passenger_Carriage$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketBooking>()
                .HasOne(ticket => ticket.Starting_Station)
                .WithMany(station => station.Ticket_Bookings_With_Station_As_Starting)
                .HasForeignKey(ticket => ticket.Starting_Station_Id)
                .HasConstraintName("Ticket_Booking@Starting_Station$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketBooking>()
                .HasOne(ticket => ticket.Ending_Station)
                .WithMany(station => station.Ticket_Bookings_With_Station_As_Ending)
                .HasForeignKey(ticket => ticket.Ending_Station_Id)
                .HasConstraintName("Ticket_Booking@Ending_Station$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketBooking>()
                .HasOne(ticket => ticket.User)
                .WithMany(user => user.Ticket_Bookings)
                .HasForeignKey(ticket => ticket.User_Id)
                .HasConstraintName("Ticket_Booking@User$FK");
            model_builder.Entity<TicketBooking>()
                .Property(ticket => ticket.Ticket_Status)
                .HasConversion<string>();
            model_builder.Entity<TicketBooking>().ToTable("Ticket_Booking");
            model_builder.Entity<TicketBooking>()
                .HasIndex(ticket => ticket.Place_In_Carriage)
                .HasDatabaseName("Ticket_Booking%Place_In_Carriage$IDX")
                .IsUnique(false);
            model_builder.Entity<TicketBooking>()
                .HasIndex(ticket => new { ticket.Place_In_Carriage, ticket.Train_Route_On_Date_Id })
                .HasDatabaseName("Ticket_Booking%Place_In_Carriage&Train_Route_On_Date_Id$IDX")
                .IsUnique(false);

            //8. Ticket_In_Selling
            model_builder.Entity<TicketInSelling>()
                .HasKey(ticket_in_selling => new
                {
                    ticket_in_selling.Train_Route_On_Date_Id,
                    ticket_in_selling.Passenger_Carriage_Id,
                    ticket_in_selling.Starting_Station_Id,
                    ticket_in_selling.Ending_Station_Id
                });
            model_builder.Entity<TicketInSelling>()
                .HasOne(ticket_in_selling => ticket_in_selling.Train_Route_On_Date)
                .WithMany(train_route_on_date => train_route_on_date.Opened_Tickets)
                .HasForeignKey(ticket_in_selling => ticket_in_selling.Train_Route_On_Date_Id)
                .HasConstraintName("Ticket_In_Selling@Train_Route_On_Date$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketInSelling>()
                .HasOne(ticket_in_selling => ticket_in_selling.Passenger_Carriage)
                .WithMany(carriage => carriage.Opened_Tickets)
                .HasForeignKey(ticket_in_selling => ticket_in_selling.Passenger_Carriage_Id)
                .HasConstraintName("Ticket_In_Selling@Passenger_Carriage$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketInSelling>()
                .HasOne(ticket_in_selling => ticket_in_selling.Starting_Station)
                .WithMany(station => station.Opened_Tickets_With_Station_As_Starting)
                .HasForeignKey(ticket_in_selling => ticket_in_selling.Starting_Station_Id)
                .HasConstraintName("Ticket_In_Selling@Starting_Station$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketInSelling>()
                .HasOne(ticket_in_selling => ticket_in_selling.Ending_Station)
                .WithMany(station => station.Opened_Tickets_With_Station_As_Ending)
                .HasForeignKey(ticket_in_selling => ticket_in_selling.Ending_Station_Id)
                .HasConstraintName("Ticket_In_Selling@Ending_Station$FK")
                .OnDelete(DeleteBehavior.Cascade);
            model_builder.Entity<TicketInSelling>().ToTable("Ticket_In_Selling");

            //9. Railway_Branch
            model_builder.Entity<RailwayBranch>().ToTable("Railway_Branch");
            //10. User
            model_builder.Entity<User>().ToTable("User");
            model_builder.Entity<User>().Property(user => user.Exemption).HasConversion<string>();
            model_builder.Entity<User>().Property(user => user.Sex).HasConversion<string>();
            model_builder.Entity<User>().Property(user => user.Role).HasConversion<string>();
            //11. User_Profile
            model_builder.Entity<UserProfile>().ToTable("User_Profile");
            model_builder.Entity<UserProfile>()
                .HasOne(user_profile => user_profile.User)
                .WithOne(user => user.User_Profile)
                .HasForeignKey<UserProfile>(user_profile => user_profile.User_Id);
            //12. Image
            model_builder.Entity<Image>().ToTable("Image");
            model_builder.Entity<Image>().Property(image => image.Image_Data).HasColumnType("MEDIUMBLOB");
            model_builder.Entity<Image>()
                .HasOne(image => image.User_Profile)
                .WithMany(user_profile => user_profile.Images)
                .HasForeignKey(image => image.User_Profile_Id);

        }
    }
}
