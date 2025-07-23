using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VacancyApplicationForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VacancyApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CoverLetter = table.Column<string>(type: "text", nullable: false),
                    ResumeUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ResumeFileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VacancyId = table.Column<int>(type: "integer", nullable: false),
                    VacancyTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyApplications_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f0c1c5d0-5002-471d-88d3-a471ef0677f2", "AQAAAAIAAYagAAAAEK9x2YRGiqpmZDrS/GEE9FNchAaxmRboKdVlaqsoxZGUsf39i7MrzfcVHwgrabX3TQ==", "1e7ff5c6-f1d2-4324-ab5f-fca3d8d5e557" });

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_CreatedAt",
                table: "VacancyApplications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_Email",
                table: "VacancyApplications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_VacancyId",
                table: "VacancyApplications",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacancyApplications");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "46d673c5-d8d6-486d-a617-badfed563914", "AQAAAAIAAYagAAAAEP3zYsmA0tzOiWnAarvOPpsSLWmKFlrJI7xp7RDT3H0H6V6VFnBBcyP5S2oiZ71r1A==", "f618b36d-0332-4f9e-a099-4d6731d13d6b" });
        }
    }
}
