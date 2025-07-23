using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
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
                    SlugUz = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SlugRu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SlugEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaTitleUz = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaTitleRu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaTitleEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaDescriptionUz = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MetaDescriptionRu = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MetaDescriptionEn = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CategoryUz = table.Column<string>(type: "text", nullable: true),
                    CategoryRu = table.Column<string>(type: "text", nullable: true),
                    CategoryEn = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SmallPhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LargePhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1", null, "SuperAdmin", "SUPERADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "50214b07-28cd-433f-98f3-494ee6173680", "admin@soskd.uz", true, false, null, "ADMIN@SOSKD.UZ", "ADMIN@SOSKD.UZ", "AQAAAAIAAYagAAAAEDmZN4I8FQsPunfUO4zwLoqsv0qCR2b/njb28iXhqZ3pFqdX2NTzxwOpEVmhXUEvBA==", null, false, "15efa566-4740-4d81-9b2d-1340bda47689", false, "admin@soskd.uz" });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "CategoryEn", "CategoryRu", "CategoryUz", "CreatedAt", "DescriptionEn", "DescriptionRu", "DescriptionUz", "LargePhotoUrl", "MetaDescriptionEn", "MetaDescriptionRu", "MetaDescriptionUz", "MetaTitleEn", "MetaTitleRu", "MetaTitleUz", "PublishedDate", "SlugEn", "SlugRu", "SlugUz", "SmallPhotoUrl", "Status", "TitleEn", "TitleRu", "TitleUz", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Education", "Образование", "Ta'lim", new DateTime(2025, 6, 21, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6528), "<p>SOS Children's Village in Tashkent has launched a new educational program. This program focuses on the social and psychological development of children.</p>", "<p>SOS Детская деревня в Ташкенте запустила новую образовательную программу. Эта программа направлена на социальное и психологическое развитие детей.</p>", "<p>SOS Bolalar shaharchasi Toshkentda yangi ta'lim dasturini boshladi. Ushbu dastur bolalarning ijtimoiy va psixologik rivojlanishiga qaratilgan.</p>", "/uploads/news/news1_large.jpg", "SOS Children's Village in Tashkent has launched a new educational program", "SOS Детская деревня в Ташкенте запустила новую образовательную программу", "SOS Bolalar shaharchasi Toshkentda yangi ta'lim dasturini boshladi", "SOS Children's Village launches new program", "SOS Детская деревня запустила новую программу", "SOS Bolalar shaharchasi yangi dasturni boshladi", new DateTime(2025, 6, 21, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6521), "new-program-launched", "novaya-programma-zapuschena", "yangi-dastur-boshlandi", "/uploads/news/news1_small.jpg", 1, "SOS Children's Village launches new program", "SOS Детская деревня запустила новую программу", "SOS Bolalar shaharchasi yangi dasturni boshladi", new DateTime(2025, 6, 21, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6529) },
                    { 2, "Events", "События", "Tadbirlar", new DateTime(2025, 6, 16, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6534), "<p>On June 1st, International Children's Day, a large festive event was held at SOS Children's Village. More than 200 children participated in the event.</p>", "<p>1 июня, в честь Международного дня детей, в SOS Детской деревне было проведено большое праздничное мероприятие. В мероприятии приняли участие более 200 детей.</p>", "<p>1-iyun, Xalqaro bolalar kuni munosabati bilan SOS Bolalar shaharchasida katta bayram tadbiri o'tkazildi. Tadbirda 200 dan ortiq bola qatnashdi.</p>", "/uploads/news/news2_large.jpg", "On June 1st, International Children's Day, a large festive event was held at SOS Children's Village", "1 июня, в честь Международного дня детей, в SOS Детской деревне было проведено большое праздничное мероприятие", "1-iyun, Xalqaro bolalar kuni munosabati bilan SOS Bolalar shaharchasida katta bayram tadbiri o'tkazildi", "International Children's Day Event", "Мероприятие в честь Международного дня детей", "Xalqaro bolalar kuni tadbiri", new DateTime(2025, 6, 16, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6534), "childrens-day-event", "den-detej-meropriyatie", "bolalar-kuni-tadbiri", "/uploads/news/news2_small.jpg", 1, "International Children's Day Event", "Мероприятие в честь Международного дня детей", "Xalqaro bolalar kuni tadbiri", new DateTime(2025, 6, 16, 18, 58, 53, 865, DateTimeKind.Utc).AddTicks(6535) }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
