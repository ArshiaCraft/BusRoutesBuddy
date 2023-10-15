using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketOffice.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Route", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 48, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    IsManager = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteCity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DepartureTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CostFromPreviousCity = table.Column<double>(type: "REAL", nullable: true),
                    RouteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteCity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteCity_Route_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PassengerFirstName = table.Column<string>(type: "TEXT", nullable: false),
                    PassengerLastName = table.Column<string>(type: "TEXT", nullable: false),
                    PassengerPlace = table.Column<int>(type: "INTEGER", nullable: false),
                    OderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RouteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_Route_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ticket_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketCity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DepartureTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CostFromPreviousCity = table.Column<double>(type: "REAL", nullable: true),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCity_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteCity_RouteId",
                table: "RouteCity",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_RouteId",
                table: "Ticket",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserId",
                table: "Ticket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCity_TicketId",
                table: "TicketCity",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteCity");

            migrationBuilder.DropTable(
                name: "TicketCity");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Route");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
