using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Soskd.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DonorEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DonorPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpayTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NextPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    ParentDonationId = table.Column<int>(type: "integer", nullable: true),
                    PaymentCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donations_Donations_ParentDonationId",
                        column: x => x.ParentDonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "039a40e6-6851-4b1a-956e-b4ee2cc007a9", "AQAAAAIAAYagAAAAEKRxUFSmRm5nOZF+DdkO3/LJ3M0AK92o8Zc1BiqW89wzt/KFjqa20ZFD2lASbwQDIg==", "d6fd17ad-bc8e-4aad-a742-75670a1c089f" });

            migrationBuilder.CreateIndex(
                name: "IX_Donations_CreatedAt",
                table: "Donations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonorEmail",
                table: "Donations",
                column: "DonorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_NextPaymentDate",
                table: "Donations",
                column: "NextPaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_OrderId",
                table: "Donations",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_ParentDonationId",
                table: "Donations",
                column: "ParentDonationId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_Status",
                table: "Donations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_Type",
                table: "Donations",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_UpayTransactionId",
                table: "Donations",
                column: "UpayTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ade6530d-f8ac-45d0-bd05-6a52ddf04ed5", "AQAAAAIAAYagAAAAEIJM6J3d0fmrlNnNxQSsgJR9Sg3np2ut+cqGgEDBOUHKEI39VAA7vQO8/GOOzofaBg==", "0eda659c-c1b1-487c-94d3-0a34df153595" });
        }
    }
}
