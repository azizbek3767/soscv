using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualFileSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "FileNameEn",
                table: "Documents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileNameUz",
                table: "Documents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytesEn",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytesUz",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FileUrlEn",
                table: "Documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUrlUz",
                table: "Documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dc34fdb6-25bf-4bac-88cc-8e6d903fb89c", "AQAAAAIAAYagAAAAELtGGe9wVT3DcGFYFgqu1z6RHnAgy6Wr4ixiGDMkdDObzvq1cA+132bGJo5fDHLNmw==", "4baa68b9-e236-4046-81a8-a27730e2ea75" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileNameEn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileNameUz",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileSizeBytesEn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileSizeBytesUz",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileUrlEn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileUrlUz",
                table: "Documents");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "039a40e6-6851-4b1a-956e-b4ee2cc007a9", "AQAAAAIAAYagAAAAEKRxUFSmRm5nOZF+DdkO3/LJ3M0AK92o8Zc1BiqW89wzt/KFjqa20ZFD2lASbwQDIg==", "d6fd17ad-bc8e-4aad-a742-75670a1c089f" });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "Category", "CategoryEn", "CategoryRu", "CategoryUz", "CreatedAt", "DescriptionEn", "DescriptionRu", "DescriptionUz", "FileName", "FileSizeBytes", "FileUrl", "PublishDate", "Status", "TitleEn", "TitleRu", "TitleUz", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Documents", "Документы", "Hujjatlar", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc), "Document defining the main activities and goals of the organization", "Документ, определяющий основную деятельность и цели организации", "Tashkilotning asosiy faoliyati va maqsadlarini belgilovchi hujjat", "sos_charter.pdf", 2048576L, "/uploads/documents/sos_charter.pdf", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc), 1, "SOS Children's Village Charter", "Устав SOS Детской деревни", "SOS Bolalar shaharchasi nizomi", new DateTime(2024, 11, 16, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "Security and Values", "Безопасность и ценности", "Xavfsizlik va qadriyatlar", new DateTime(2024, 11, 23, 10, 0, 0, 0, DateTimeKind.Utc), "Policy document on child safety and protection", "Документ политики по безопасности и защите детей", "Bolalar xavfsizligi va muhofazasi bo'yicha siyosat hujjati", "security_policy.pdf", 1572864L, "/uploads/documents/security_policy.pdf", new DateTime(2024, 11, 23, 10, 0, 0, 0, DateTimeKind.Utc), 1, "Security Policy", "Политика безопасности", "Xavfsizlik siyosati", new DateTime(2024, 11, 23, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, "Documents", "Документы", "Hujjatlar", new DateTime(2024, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc), "SOS Children's Village activity report for 2023", "Отчет о деятельности SOS Детской деревни за 2023 год", "SOS Bolalar shaharchasining 2023 yilgi faoliyat hisoboti", "annual_report_2023.pdf", 5242880L, "/uploads/documents/annual_report_2023.pdf", new DateTime(2024, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "Annual Report 2023", "Годовой отчет 2023", "Yillik hisobot 2023", new DateTime(2024, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
