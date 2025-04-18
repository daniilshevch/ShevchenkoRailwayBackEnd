using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class TicketStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "Ticket_Booking");

            migrationBuilder.AddColumn<int>(
                name: "Ticket_Status",
                table: "Ticket_Booking",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ticket_Status",
                table: "Ticket_Booking");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "Ticket_Booking",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
