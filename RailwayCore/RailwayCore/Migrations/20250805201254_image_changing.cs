using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class image_changing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_User_Profile_User_ProfileId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_User_ProfileId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "User_ProfileId",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "Image");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image",
                table: "Image",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Image_User_Profile_Id",
                table: "Image",
                column: "User_Profile_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_User_Profile_User_Profile_Id",
                table: "Image",
                column: "User_Profile_Id",
                principalTable: "User_Profile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_User_Profile_User_Profile_Id",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_User_Profile_Id",
                table: "Image");

            migrationBuilder.RenameTable(
                name: "Image",
                newName: "Images");

            migrationBuilder.AddColumn<int>(
                name: "User_ProfileId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Images_User_ProfileId",
                table: "Images",
                column: "User_ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_User_Profile_User_ProfileId",
                table: "Images",
                column: "User_ProfileId",
                principalTable: "User_Profile",
                principalColumn: "Id");
        }
    }
}
