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
                    statid = table.Column<int>(type: "int", nullable: false),
                    entity = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    starttime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endtime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    lat = table.Column<double>(type: "float", nullable: true),
                    @long = table.Column<double>(name: "long", type: "float", nullable: true),
                    height = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => new { x.statid, x.starttime, x.entity });
                    table.ForeignKey(
                        name: "FK_Position_Stations_statid",
                        column: x => x.statid,
                        principalTable: "Stations",
                        principalColumn: "statid",
                        onDelete: ReferentialAction.Cascade);
                });
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
