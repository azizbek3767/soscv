// Soskd.Infrastructure/Data/Configurations/DonationConfiguration.cs - Updated for PostgreSQL
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;

namespace Soskd.Infrastructure.Data.Configurations
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("donations"); // PostgreSQL convention: lowercase table names

            builder.HasKey(d => d.Id);

            builder.Property(d => d.DonorName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.DonorEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.DonorPhone)
                .HasMaxLength(20);

            // PostgreSQL decimal type
            builder.Property(d => d.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(d => d.ProcessedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(d => d.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(DonationStatus.Pending);

            builder.Property(d => d.OrderId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.UpayTransactionId)
                .HasMaxLength(100);

            builder.Property(d => d.PaymentMethod)
                .HasMaxLength(500);

            builder.Property(d => d.FailureReason)
                .HasMaxLength(1000);

            builder.Property(d => d.IpAddress)
                .HasMaxLength(45);

            builder.Property(d => d.UserAgent)
                .HasMaxLength(500);

            // PostgreSQL timestamp with timezone
            builder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            builder.Property(d => d.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            builder.Property(d => d.NextPaymentDate)
                .HasColumnType("timestamp with time zone");

            builder.Property(d => d.PaymentCompletedAt)
                .HasColumnType("timestamp with time zone");

            // Indexes for performance (PostgreSQL naming convention)
            builder.HasIndex(d => d.OrderId)
                .IsUnique()
                .HasDatabaseName("ix_donations_order_id");

            builder.HasIndex(d => d.UpayTransactionId)
                .HasDatabaseName("ix_donations_upay_transaction_id");

            builder.HasIndex(d => d.Status)
                .HasDatabaseName("ix_donations_status");

            builder.HasIndex(d => d.Type)
                .HasDatabaseName("ix_donations_type");

            builder.HasIndex(d => d.CreatedAt)
                .HasDatabaseName("ix_donations_created_at");

            builder.HasIndex(d => d.DonorEmail)
                .HasDatabaseName("ix_donations_donor_email");

            // Self-referencing relationship for recurring payments
            builder.HasOne(d => d.ParentDonation)
                .WithMany(d => d.RecurringPayments)
                .HasForeignKey(d => d.ParentDonationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}