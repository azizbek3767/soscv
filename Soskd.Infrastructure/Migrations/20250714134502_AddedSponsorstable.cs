using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSponsorstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleUz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TitleRu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "46d673c5-d8d6-486d-a617-badfed563914", "AQAAAAIAAYagAAAAEP3zYsmA0tzOiWnAarvOPpsSLWmKFlrJI7xp7RDT3H0H6V6VFnBBcyP5S2oiZ71r1A==", "f618b36d-0332-4f9e-a099-4d6731d13d6b" });

            migrationBuilder.InsertData(
                table: "Sponsors",
                columns: new[] { "Id", "CreatedAt", "DisplayOrder", "Link", "PhotoUrl", "Status", "TitleEn", "TitleRu", "TitleUz", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 11, 10, 0, 0, 0, DateTimeKind.Utc), 1, "https://www.microsoft.com", "/uploads/sponsors/microsoft_logo.png", 1, "Microsoft Corporation", "Корпорация Майкрософт", "Microsoft Corporation", new DateTime(2024, 11, 11, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 11, 13, 10, 0, 0, 0, DateTimeKind.Utc), 2, "https://www.google.com", "/uploads/sponsors/google_logo.png", 1, "Google LLC", "Гугл ЛЛК", "Google LLC", new DateTime(2024, 11, 13, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc), 3, "https://www.apple.com", "/uploads/sponsors/apple_logo.png", 1, "Apple Inc.", "Эппл Инк.", "Apple Inc.", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2024, 11, 21, 10, 0, 0, 0, DateTimeKind.Utc), 4, "https://www.meta.com", "/uploads/sponsors/meta_logo.png", 2, "Meta Platforms", "Мета Платформс", "Meta Platforms", new DateTime(2024, 11, 21, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_CreatedAt",
                table: "Sponsors",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_DisplayOrder",
                table: "Sponsors",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_Status",
                table: "Sponsors",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3dd178d-67a9-45b2-87f4-995c8679fe6b", "AQAAAAIAAYagAAAAEGVSHG/jOwLISLb2EeMUE35iDYp/qL5Z0qrjcZ9uAJ82kh/ve90Z1Y09IQFViN2PVg==", "6b41469a-e12c-436f-a1e2-f7dda00873d3" });
        }
    }
}
