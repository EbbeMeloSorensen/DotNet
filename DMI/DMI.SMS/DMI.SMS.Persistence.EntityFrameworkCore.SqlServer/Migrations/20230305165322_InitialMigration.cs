using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMI.SMS.Persistence.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StationInformations",
                columns: table => new
                {
                    gdb_archive_oid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stationname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stationid_dmi = table.Column<int>(type: "int", nullable: true),
                    stationtype = table.Column<int>(type: "int", nullable: true),
                    accessaddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    country = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true),
                    datefrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dateto = table.Column<DateTime>(type: "datetime2", nullable: true),
                    stationOwner = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    stationid_icao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    referencetomaintenanceagreement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    facilityid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    si_utm = table.Column<int>(type: "int", nullable: true),
                    si_northing = table.Column<double>(type: "float", nullable: true),
                    si_easting = table.Column<double>(type: "float", nullable: true),
                    si_geo_lat = table.Column<double>(type: "float", nullable: true),
                    si_geo_long = table.Column<double>(type: "float", nullable: true),
                    serviceinterval = table.Column<int>(type: "int", nullable: true),
                    lastservicedate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nextservicedate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    addworkforcedate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    lastvisitdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    altstationid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wmostationid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    regionid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wigosid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wmocountrycode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hha = table.Column<double>(type: "float", nullable: true),
                    hhp = table.Column<double>(type: "float", nullable: true),
                    wmorbsn = table.Column<int>(type: "int", nullable: true),
                    wmorbcn = table.Column<int>(type: "int", nullable: true),
                    wmorbsnradio = table.Column<int>(type: "int", nullable: true),
                    wgs_lat = table.Column<double>(type: "float", nullable: true),
                    wgs_long = table.Column<double>(type: "float", nullable: true),
                    Shape = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    globalid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    objectid = table.Column<int>(type: "int", nullable: false),
                    created_user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    last_edited_user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_edited_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gdb_from_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gdb_to_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationInformations", x => x.gdb_archive_oid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationInformations");
        }
    }
}
