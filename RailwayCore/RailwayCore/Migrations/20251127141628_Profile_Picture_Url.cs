using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class Profile_Picture_Url : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Profile_Picture_Url",
                table: "User_Profile",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile_Picture_Url",
                table: "User_Profile");
        }
    }
}
