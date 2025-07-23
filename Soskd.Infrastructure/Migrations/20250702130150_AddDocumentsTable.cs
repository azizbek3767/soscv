using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleUz = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TitleRu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DescriptionUz = table.Column<string>(type: "text", nullable: true),
                    DescriptionRu = table.Column<string>(type: "text", nullable: true),
                    DescriptionEn = table.Column<string>(type: "text", nullable: true),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CategoryUz = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CategoryRu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CategoryEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3dd178d-67a9-45b2-87f4-995c8679fe6b", "AQAAAAIAAYagAAAAEGVSHG/jOwLISLb2EeMUE35iDYp/qL5Z0qrjcZ9uAJ82kh/ve90Z1Y09IQFViN2PVg==", "6b41469a-e12c-436f-a1e2-f7dda00873d3" });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "Category", "CategoryEn", "CategoryRu", "CategoryUz", "CreatedAt", "DescriptionEn", "DescriptionRu", "DescriptionUz", "FileName", "FileSizeBytes", "FileUrl", "PublishDate", "Status", "TitleEn", "TitleRu", "TitleUz", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Documents", "Документы", "Hujjatlar", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc), "Document defining the main activities and goals of the organization", "Документ, определяющий основную деятельность и цели организации", "Tashkilotning asosiy faoliyati va maqsadlarini belgilovchi hujjat", "sos_charter.pdf", 2048576L, "/uploads/documents/sos_charter.pdf", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc), 1, "SOS Children's Village Charter", "Устав SOS Детской деревни", "SOS Bolalar shaharchasi nizomi", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "Security and Values", "Безопасность и ценности", "Xavfsizlik va qadriyatlar", new DateTime(2024, 11, 23, 10, 0, 0, 0, DateTimeKind.Utc), "Policy document on child safety and protection", "Документ политики по безопасности и защите детей", "Bolalar xavfsizligi va muhofazasi bo'yicha siyosat hujjati", "security_policy.pdf", 1572864L, "/uploads/documents/security_policy.pdf", new DateTime(2024, 11, 23, 10, 0, 0, 0, DateTimeKind.Utc), 1, "Security Policy", "Политика безопасности", "Xavfsizlik siyosati", new DateTime(2024, 11, 23, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, "Documents", "Документы", "Hujjatlar", new DateTime(2024, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc), "SOS Children's Village activity report for 2023", "Отчет о деятельности SOS Детской деревни за 2023 год", "SOS Bolalar shaharchasining 2023 yilgi faoliyat hisoboti", "annual_report_2023.pdf", 5242880L, "/uploads/documents/annual_report_2023.pdf", new DateTime(2024, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "Annual Report 2023", "Годовой отчет 2023", "Yillik hisobot 2023", new DateTime(2024, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Category",
                table: "Documents",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedAt",
                table: "Documents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PublishDate",
                table: "Documents",
                column: "PublishDate");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Status",
                table: "Documents",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e2be014c-878a-43c1-a8f2-d829c5390123", "AQAAAAIAAYagAAAAEIsBtIiboSrp9ik9BqgggApFaUrOvI2vx3LpKjvbbA03TlT1JWXl2dGGYFHYgQWn5w==", "4bc2818a-6683-479b-86f2-b99c801548ee" });
        }
    }
}
