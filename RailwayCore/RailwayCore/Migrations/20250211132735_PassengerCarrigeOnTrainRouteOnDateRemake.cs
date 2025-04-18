using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class PassengerCarrigeOnTrainRouteOnDateRemake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Factual_Air_Conditioning",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Factual_Shower_Availability",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Factural_Is_Inclusive",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Food_Availability",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is_For_Children",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is_For_Woman",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Factual_Air_Conditioning",
                table: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Factual_Shower_Availability",
                table: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Factural_Is_Inclusive",
                table: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Food_Availability",
                table: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Is_For_Children",
                table: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Is_For_Woman",
                table: "Passenger_Carriage_On_Train_Route_On_Date");
        }
    }
}
