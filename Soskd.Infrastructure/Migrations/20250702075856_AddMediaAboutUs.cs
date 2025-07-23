using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaAboutUs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaAboutUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleUz = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TitleRu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DescriptionUz = table.Column<string>(type: "text", nullable: false),
                    DescriptionRu = table.Column<string>(type: "text", nullable: false),
                    DescriptionEn = table.Column<string>(type: "text", nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SourceLink = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAboutUs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e2be014c-878a-43c1-a8f2-d829c5390123", "AQAAAAIAAYagAAAAEIsBtIiboSrp9ik9BqgggApFaUrOvI2vx3LpKjvbbA03TlT1JWXl2dGGYFHYgQWn5w==", "4bc2818a-6683-479b-86f2-b99c801548ee" });

            migrationBuilder.InsertData(
                table: "MediaAboutUs",
                columns: new[] { "Id", "CreatedAt", "DescriptionEn", "DescriptionRu", "DescriptionUz", "PhotoUrl", "PublishDate", "SourceLink", "Status", "TitleEn", "TitleRu", "TitleUz", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 24, 10, 0, 0, 0, DateTimeKind.Utc), "<p>Local press published a detailed article about the activities of SOS Children's Village. The article emphasizes the importance of the organization in children's lives.</p>", "<p>Местная пресса опубликовала подробную статью о деятельности SOS Детской деревни. В статье подчеркивается важность организации в жизни детей.</p>", "<p>Mahalliy matbuot SOS Bolalar shaharchasining faoliyati haqida batafsil maqola chop etdi. Maqolada tashkilotning bolalar hayotidagi ahamiyati ta'kidlangan.</p>", "/uploads/media/media1.jpg", new DateTime(2024, 11, 24, 10, 0, 0, 0, DateTimeKind.Utc), "https://kun.uz/news/2024/11/15/sos-bolalar-shaharchasi", 1, "New article about SOS Children's Village", "Новая статья о SOS Детской деревне", "SOS Bolalar shaharchasi haqida yangi maqola", new DateTime(2024, 11, 24, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 11, 19, 10, 0, 0, 0, DateTimeKind.Utc), "<p>A special report about new programs of SOS Children's Village was broadcasted on national television.</p>", "<p>На национальном телевидении был показан специальный репортаж о новых программах SOS Детской деревни.</p>", "<p>Milliy televidenie kanalida SOS Bolalar shaharchasining yangi dasturlari haqida maxsus reportaj efirga uzatildi.</p>", "/uploads/media/media2.jpg", new DateTime(2024, 11, 19, 10, 0, 0, 0, DateTimeKind.Utc), "https://uzreport.news/society/sos-bolalar-shaharchasi-yangi-dasturlar", 1, "Television coverage of SOS programs", "Телевидение о программах SOS", "Televidenie SOS dasturlari haqida", new DateTime(2024, 11, 19, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAboutUs_CreatedAt",
                table: "MediaAboutUs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAboutUs_PublishDate",
                table: "MediaAboutUs",
                column: "PublishDate");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAboutUs_Status",
                table: "MediaAboutUs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaAboutUs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e2f1d05-2622-4536-b2eb-6aed375a4cd5", "AQAAAAIAAYagAAAAEPfvU7NHD1ypE/fXIvAEIo4CyZlTHyPlIX3favr0fCS2lFhDn+5j013xOwU3No0btQ==", "d426b23a-5d15-45b8-a01d-cc1ee6fa31ae" });
        }
    }
}
