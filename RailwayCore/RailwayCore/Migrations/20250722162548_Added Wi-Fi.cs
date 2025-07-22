using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class AddedWiFi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Factual_Wi_Fi",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Wi_Fi",
                table: "Passenger_Carriage",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Factual_Wi_Fi",
                table: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Wi_Fi",
                table: "Passenger_Carriage");
        }
    }
}
