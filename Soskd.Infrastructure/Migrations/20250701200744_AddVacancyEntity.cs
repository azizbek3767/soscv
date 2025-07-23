using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVacancyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleUz = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DescriptionUz = table.Column<string>(type: "text", nullable: false),
                    TitleRu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DescriptionRu = table.Column<string>(type: "text", nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DescriptionEn = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e2f1d05-2622-4536-b2eb-6aed375a4cd5", "AQAAAAIAAYagAAAAEPfvU7NHD1ypE/fXIvAEIo4CyZlTHyPlIX3favr0fCS2lFhDn+5j013xOwU3No0btQ==", "d426b23a-5d15-45b8-a01d-cc1ee6fa31ae" });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PublishedDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 11, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 11, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 11, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PublishedDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 11, 21, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 11, 21, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 11, 21, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Vacancies",
                columns: new[] { "Id", "CreatedAt", "Deadline", "DescriptionEn", "DescriptionRu", "DescriptionUz", "PublishedDate", "Status", "TitleEn", "TitleRu", "TitleUz", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 28, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 15, 10, 0, 0, 0, DateTimeKind.Utc), "<p>SOS Children's Village is looking for a Social Worker. The candidate should have experience working with children.</p><p><strong>Requirements:</strong></p><ul><li>Social work or psychology degree</li><li>Minimum 2 years of experience</li><li>Knowledge of Uzbek and Russian languages</li></ul>", "<p>SOS Детская деревня ищет сотрудника на должность социального работника. Кандидат должен иметь опыт работы с детьми.</p><p><strong>Требования:</strong></p><ul><li>Специальность социальная работа или психология</li><li>Минимум 2 года опыта работы</li><li>Знание узбекского и русского языков</li></ul>", "<p>SOS Bolalar shaharchasi ijtimoiy ishchi lavozimiga xodim izlaydi. Nomzod bolalar bilan ishlash tajribasiga ega bo'lishi kerak.</p><p><strong>Talablar:</strong></p><ul><li>Ijtimoiy ish yoki psixologiya mutaxassisligi</li><li>Kamida 2 yil ish tajribasi</li><li>O'zbek va rus tillarini bilish</li></ul>", new DateTime(2024, 11, 28, 10, 0, 0, 0, DateTimeKind.Utc), 2, "Social Worker", "Социальный работник", "Ijtimoiy ishchi", new DateTime(2024, 11, 28, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 11, 30, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 22, 10, 0, 0, 0, DateTimeKind.Utc), "<p>An Education Coordinator is needed to plan and implement educational programs.</p><p><strong>Responsibilities:</strong></p><ul><li>Develop educational programs</li><li>Work with teachers</li><li>Prepare reports</li></ul>", "<p>Требуется координатор по планированию и реализации образовательных программ.</p><p><strong>Обязанности:</strong></p><ul><li>Разработка образовательных программ</li><li>Работа с учителями</li><li>Подготовка отчетов</li></ul>", "<p>Ta'lim dasturlarini rejalashtirish va amalga oshirish bo'yicha koordinator zarur.</p><p><strong>Mas'uliyatlar:</strong></p><ul><li>Ta'lim dasturlarini ishlab chiqish</li><li>O'qituvchilar bilan ishlash</li><li>Hisobotlar tayyorlash</li></ul>", new DateTime(2024, 11, 30, 10, 0, 0, 0, DateTimeKind.Utc), 2, "Education Coordinator", "Координатор по образованию", "Ta'lim koordinatori", new DateTime(2024, 11, 30, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CreatedAt",
                table: "Vacancies",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_Deadline",
                table: "Vacancies",
                column: "Deadline");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_PublishedDate",
                table: "Vacancies",
                column: "PublishedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_Status",
                table: "Vacancies",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "50214b07-28cd-433f-98f3-494ee6173680", "AQAAAAIAAYagAAAAEDmZN4I8FQsPunfUO4zwLoqsv0qCR2b/njb28iXhqZ3pFqdX2NTzxwOpEVmhXUEvBA==", "15efa566-4740-4d81-9b2d-1340bda47689" });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PublishedDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 21, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6528), new DateTime(2025, 6, 21, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6521), new DateTime(2025, 6, 21, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6529) });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PublishedDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6534), new DateTime(2025, 6, 16, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6534), new DateTime(2025, 6, 16, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6535) });
        }
    }
}
