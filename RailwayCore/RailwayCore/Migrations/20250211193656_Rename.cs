using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Factural_Is_Inclusive",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                newName: "Factual_Is_Inclusive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Factual_Is_Inclusive",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                newName: "Factural_Is_Inclusive");
        }
    }
}
