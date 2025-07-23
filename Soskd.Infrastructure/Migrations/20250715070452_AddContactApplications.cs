using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactApplications", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ade6530d-f8ac-45d0-bd05-6a52ddf04ed5", "AQAAAAIAAYagAAAAEIJM6J3d0fmrlNnNxQSsgJR9Sg3np2ut+cqGgEDBOUHKEI39VAA7vQO8/GOOzofaBg==", "0eda659c-c1b1-487c-94d3-0a34df153595" });

            migrationBuilder.InsertData(
                table: "ContactApplications",
                columns: new[] { "Id", "City", "CreatedAt", "Email", "FullName", "IpAddress", "Message", "Phone", "UserAgent" },
                values: new object[,]
                {
                    { 1, "Tashkent", new DateTime(2024, 11, 26, 10, 0, 0, 0, DateTimeKind.Utc), "john.doe@example.com", "John Doe", "192.168.1.100", "I would like to learn more about your programs and how I can help support the children.", "+998901234567", null },
                    { 2, "Samarkand", new DateTime(2024, 11, 29, 10, 0, 0, 0, DateTimeKind.Utc), "maria.petrova@example.com", "Maria Petrova", "192.168.1.101", "Hello, I am interested in volunteering opportunities. Could you please provide more information about how to get involved?", "+998907654321", null },
                    { 3, "Bukhara", new DateTime(2024, 11, 30, 10, 0, 0, 0, DateTimeKind.Utc), "ahmad.karimov@example.com", "Ahmad Karimov", "192.168.1.102", "Salom! Men SOS Bolalar shaharchasi haqida ko'proq ma'lumot olishni xohlayman. Qanday yordam bera olaman?", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactApplications_City",
                table: "ContactApplications",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_ContactApplications_CreatedAt",
                table: "ContactApplications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContactApplications_Email",
                table: "ContactApplications",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactApplications");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f0c1c5d0-5002-471d-88d3-a471ef0677f2", "AQAAAAIAAYagAAAAEK9x2YRGiqpmZDrS/GEE9FNchAaxmRboKdVlaqsoxZGUsf39i7MrzfcVHwgrabX3TQ==", "1e7ff5c6-f1d2-4324-ab5f-fca3d8d5e557" });
        }
    }
}
