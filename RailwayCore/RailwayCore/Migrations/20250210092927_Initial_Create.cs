using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayCore.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Railway_Branch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Office_Location = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Railway_Branch", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User_Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Register_Id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type_Of = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Region = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Locomotive_Depot = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Carriage_Depot = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Railway_Branch_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Id);
                    table.ForeignKey(
                        name: "Station@Railway_Branch$FK",
                        column: x => x.Railway_Branch_Id,
                        principalTable: "Railway_Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Train_Route",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Is_Branded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Quality_Class = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Branded_Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Speed_Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Frequency_Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Assignement_Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Railway_Branch_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Train_Route", x => x.Id);
                    table.ForeignKey(
                        name: "Train_Route@Railway_Branch$FK",
                        column: x => x.Railway_Branch_Id,
                        principalTable: "Railway_Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Passenger_Carriage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type_Of = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Production_Year = table.Column<int>(type: "int", nullable: true),
                    Manufacturer = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quality_Class = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Renewal_Fact = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Renewal_Year = table.Column<int>(type: "int", nullable: true),
                    Renewal_Performer = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Renewal_Info = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Air_Conditioning = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Is_Inclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Is_For_Women = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Is_For_Children = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Is_For_Train_Chief = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Shower_Availability = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    In_Current_Use = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Appearence = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Station_Depot_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passenger_Carriage", x => x.Id);
                    table.ForeignKey(
                        name: "Passenger_Carriage@Station_Depot$FK",
                        column: x => x.Station_Depot_Id,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Train_Route_On_Date",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Train_Route_Id = table.Column<string>(type: "varchar(8)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Departure_Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Train_Route_On_Date", x => x.Id);
                    table.ForeignKey(
                        name: "Train_Route_On_Date@Train_Route$FK",
                        column: x => x.Train_Route_Id,
                        principalTable: "Train_Route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Passenger_Carriage_On_Train_Route_On_Date",
                columns: table => new
                {
                    Passenger_Carriage_Id = table.Column<string>(type: "varchar(8)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Train_Route_On_Date_Id = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Position_In_Squad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passenger_Carriage_On_Train_Route_On_Date", x => new { x.Passenger_Carriage_Id, x.Train_Route_On_Date_Id });
                    table.ForeignKey(
                        name: "Passenger_Carriage_On_Train_Route_On_Date@Passenger_Carriage$FK",
                        column: x => x.Passenger_Carriage_Id,
                        principalTable: "Passenger_Carriage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Passenger_Carriage_On_Train_Route_On_Date@Train_Route_On_Date$FK",
                        column: x => x.Train_Route_On_Date_Id,
                        principalTable: "Train_Route_On_Date",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ticket_Booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Train_Route_On_Date_Id = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Passenger_Carriage_Id = table.Column<string>(type: "varchar(8)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Starting_Station_Id = table.Column<int>(type: "int", nullable: false),
                    Ending_Station_Id = table.Column<int>(type: "int", nullable: false),
                    Place_In_Carriage = table.Column<int>(type: "int", nullable: false),
                    Passenger_Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Passenger_Surname = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Booking_Time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Is_Active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "Ticket_Booking@Ending_Station$FK",
                        column: x => x.Ending_Station_Id,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_Booking@Passenger_Carriage$FK",
                        column: x => x.Passenger_Carriage_Id,
                        principalTable: "Passenger_Carriage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_Booking@Starting_Station$FK",
                        column: x => x.Starting_Station_Id,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_Booking@Train_Route_On_Date$FK",
                        column: x => x.Train_Route_On_Date_Id,
                        principalTable: "Train_Route_On_Date",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_Booking@User$FK",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ticket_In_Selling",
                columns: table => new
                {
                    Passenger_Carriage_Id = table.Column<string>(type: "varchar(8)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Train_Route_On_Date_Id = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Starting_Station_Id = table.Column<int>(type: "int", nullable: false),
                    Ending_Station_Id = table.Column<int>(type: "int", nullable: false),
                    Extra_Coefficient = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket_In_Selling", x => new { x.Train_Route_On_Date_Id, x.Passenger_Carriage_Id, x.Starting_Station_Id, x.Ending_Station_Id });
                    table.ForeignKey(
                        name: "Ticket_In_Selling@Ending_Station$FK",
                        column: x => x.Ending_Station_Id,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_In_Selling@Passenger_Carriage$FK",
                        column: x => x.Passenger_Carriage_Id,
                        principalTable: "Passenger_Carriage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_In_Selling@Starting_Station$FK",
                        column: x => x.Starting_Station_Id,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Ticket_In_Selling@Train_Route_On_Date$FK",
                        column: x => x.Train_Route_On_Date_Id,
                        principalTable: "Train_Route_On_Date",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Train_Route_On_Date_On_Station",
                columns: table => new
                {
                    Train_Route_On_Date_Id = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Station_Id = table.Column<int>(type: "int", nullable: false),
                    Arrival_Time = table.Column<DateTime>(type: "DATETIME(0)", nullable: true),
                    Departure_Time = table.Column<DateTime>(type: "DATETIME(0)", nullable: true),
                    Stop_Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Distance_From_Starting_Station = table.Column<double>(type: "double", nullable: true),
                    Speed_On_Section = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Train_Route_On_Date_On_Station", x => new { x.Train_Route_On_Date_Id, x.Station_Id });
                    table.ForeignKey(
                        name: "Train_Route_On_Date_On_Station@Station$FK",
                        column: x => x.Station_Id,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Train_Route_On_Date_On_Station@Train_Route_On_Date$FK",
                        column: x => x.Train_Route_On_Date_Id,
                        principalTable: "Train_Route_On_Date",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_Carriage_Station_Depot_Id",
                table: "Passenger_Carriage",
                column: "Station_Depot_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_Carriage_On_Train_Route_On_Date_Train_Route_On_Dat~",
                table: "Passenger_Carriage_On_Train_Route_On_Date",
                column: "Train_Route_On_Date_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Station_Railway_Branch_Id",
                table: "Station",
                column: "Railway_Branch_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Booking_Ending_Station_Id",
                table: "Ticket_Booking",
                column: "Ending_Station_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Booking_Passenger_Carriage_Id",
                table: "Ticket_Booking",
                column: "Passenger_Carriage_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Booking_Starting_Station_Id",
                table: "Ticket_Booking",
                column: "Starting_Station_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Booking_Train_Route_On_Date_Id",
                table: "Ticket_Booking",
                column: "Train_Route_On_Date_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_Booking_User_Id",
                table: "Ticket_Booking",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_In_Selling_Ending_Station_Id",
                table: "Ticket_In_Selling",
                column: "Ending_Station_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_In_Selling_Passenger_Carriage_Id",
                table: "Ticket_In_Selling",
                column: "Passenger_Carriage_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_In_Selling_Starting_Station_Id",
                table: "Ticket_In_Selling",
                column: "Starting_Station_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Train_Route_Railway_Branch_Id",
                table: "Train_Route",
                column: "Railway_Branch_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Train_Route_On_Date_Train_Route_Id",
                table: "Train_Route_On_Date",
                column: "Train_Route_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Train_Route_On_Date_On_Station_Station_Id",
                table: "Train_Route_On_Date_On_Station",
                column: "Station_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passenger_Carriage_On_Train_Route_On_Date");

            migrationBuilder.DropTable(
                name: "Ticket_Booking");

            migrationBuilder.DropTable(
                name: "Ticket_In_Selling");

            migrationBuilder.DropTable(
                name: "Train_Route_On_Date_On_Station");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Passenger_Carriage");

            migrationBuilder.DropTable(
                name: "Train_Route_On_Date");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "Train_Route");

            migrationBuilder.DropTable(
                name: "Railway_Branch");
        }
    }
}
