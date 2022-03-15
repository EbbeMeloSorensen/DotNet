using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    statid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    icao_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    source = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.statid);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    StatID = table.Column<int>(type: "int", nullable: false),
                    entity = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    starttime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StationStatID = table.Column<int>(type: "int", nullable: false),
                    endtime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Long = table.Column<double>(type: "float", nullable: true),
                    Height = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => new { x.StatID, x.starttime, x.entity });
                    table.ForeignKey(
                        name: "FK_Position_Stations_StationStatID",
                        column: x => x.StationStatID,
                        principalTable: "Stations",
                        principalColumn: "statid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Position_StationStatID",
                table: "Position",
                column: "StationStatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
