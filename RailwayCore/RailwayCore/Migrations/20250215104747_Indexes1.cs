using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class Indexes1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "Ticket_Booking%Place_In_Carriage&Train_Route_On_Date_Id$IDX",
                table: "Ticket_Booking",
                columns: new[] { "Place_In_Carriage", "Train_Route_On_Date_Id" });

            migrationBuilder.CreateIndex(
                name: "Ticket_Booking%Place_In_Carriage$IDX",
                table: "Ticket_Booking",
                column: "Place_In_Carriage");

            migrationBuilder.CreateIndex(
                name: "Station%Title$IDX",
                table: "Station",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Ticket_Booking%Place_In_Carriage&Train_Route_On_Date_Id$IDX",
                table: "Ticket_Booking");

            migrationBuilder.DropIndex(
                name: "Ticket_Booking%Place_In_Carriage$IDX",
                table: "Ticket_Booking");

            migrationBuilder.DropIndex(
                name: "Station%Title$IDX",
                table: "Station");
        }
    }
}
