using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Term = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordAssociations_Records_ObjectRecordId",
                        column: x => x.ObjectRecordId,
                        principalTable: "Records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordAssociations_Records_SubjectRecordId",
                        column: x => x.SubjectRecordId,
                        principalTable: "Records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordAssociations_ObjectRecordId",
                table: "RecordAssociations",
                column: "ObjectRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordAssociations_SubjectRecordId",
                table: "RecordAssociations",
                column: "SubjectRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordAssociations");

            migrationBuilder.DropTable(
                name: "Records");
        }
    }
}
