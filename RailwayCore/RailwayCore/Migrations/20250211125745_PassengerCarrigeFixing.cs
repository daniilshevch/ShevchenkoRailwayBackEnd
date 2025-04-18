using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class PassengerCarrigeFixing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_For_Children",
                table: "Passenger_Carriage");

            migrationBuilder.DropColumn(
                name: "Is_For_Women",
                table: "Passenger_Carriage");

            migrationBuilder.AlterColumn<int>(
                name: "Quality_Class",
                table: "Passenger_Carriage",
                type: "int",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Quality_Class",
                table: "Passenger_Carriage",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 30,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Is_For_Children",
                table: "Passenger_Carriage",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is_For_Women",
                table: "Passenger_Carriage",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
