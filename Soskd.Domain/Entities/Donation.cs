// Soskd.Domain/Entities/Donation.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;

namespace Soskd.Domain.Entities
{
    public class Donation
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string DonorEmail { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? DonorPhone { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DonationType Type { get; set; } // OneTime, Monthly

        [Required]
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        [Required]
        [MaxLength(50)]
        public string OrderId { get; set; } = string.Empty; // Our internal order ID

        [MaxLength(100)]
        public string? UpayTransactionId { get; set; } // UPay transaction ID

        [MaxLength(500)]
        public string? PaymentMethod { get; set; } // upay, card, etc.

        [MaxLength(1000)]
        public string? FailureReason { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        // For monthly donations
        public DateTime? NextPaymentDate { get; set; }
        public bool IsRecurring { get; set; } = false;
        public int? ParentDonationId { get; set; } // For recurring payments
        public Donation? ParentDonation { get; set; }
        public ICollection<Donation> RecurringPayments { get; set; } = new List<Donation>();

        // Payment processing details
        public DateTime? PaymentCompletedAt { get; set; }
        public decimal? ProcessedAmount { get; set; }

        // Audit fields
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
