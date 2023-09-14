using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C2IEDM.Web.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LineObjectId",
                table: "LinePoints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PointObjectId",
                table: "LinePoints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineObjectId",
                table: "LinePoints");

            migrationBuilder.DropColumn(
                name: "PointObjectId",
                table: "LinePoints");
        }
    }
}
