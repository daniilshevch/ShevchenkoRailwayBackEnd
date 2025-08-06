using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class update_user_profile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "User_Id",
                table: "User_Profile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_User_Profile_User_Id",
                table: "User_Profile",
                column: "User_Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Profile_User_User_Id",
                table: "User_Profile",
                column: "User_Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Profile_User_User_Id",
                table: "User_Profile");

            migrationBuilder.DropIndex(
                name: "IX_User_Profile_User_Id",
                table: "User_Profile");

            migrationBuilder.DropColumn(
                name: "User_Id",
                table: "User_Profile");
        }
    }
}
