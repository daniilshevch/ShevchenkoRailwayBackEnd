using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class TicketServices2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Additional_Services",
                table: "Ticket_Booking",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Additional_Services",
                table: "Ticket_Booking");
        }
    }
}
