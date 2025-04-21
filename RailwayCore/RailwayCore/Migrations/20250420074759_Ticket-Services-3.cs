using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class TicketServices3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Additional_Services",
                table: "Ticket_Booking",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ticket_Booking",
                keyColumn: "Additional_Services",
                keyValue: null,
                column: "Additional_Services",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Additional_Services",
                table: "Ticket_Booking",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
