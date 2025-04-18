using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class Train_Coefficients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Train_Race_Coefficient",
                table: "Train_Route_On_Date",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Train_Route_Coefficient",
                table: "Train_Route",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Train_Race_Coefficient",
                table: "Train_Route_On_Date");

            migrationBuilder.DropColumn(
                name: "Train_Route_Coefficient",
                table: "Train_Route");
        }
    }
}
