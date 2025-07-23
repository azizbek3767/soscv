using Soskd.Domain.Enums;

namespace Soskd.Admin.ViewModels
{
    public class DonationViewModel
    {
        public int Id { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public string? DonorPhone { get; set; }
        public decimal Amount { get; set; }
        public DonationType Type { get; set; }
        public DonationStatus Status { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string? UpayTransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? PaymentCompletedAt { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public bool IsRecurring { get; set; }
        public int? ParentDonationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Display properties
        public string TypeDisplay => Type.ToString();
        public string StatusDisplay => Status.ToString();
        public string StatusBadgeClass => Status switch
        {
            DonationStatus.Completed => "bg-success",
            DonationStatus.Pending => "bg-warning",
            DonationStatus.Failed => "bg-danger",
            DonationStatus.Cancelled => "bg-secondary",
            DonationStatus.Refunded => "bg-info",
            _ => "bg-secondary"
        };
    }

    public class DonationListViewModel
    {
        public List<DonationViewModel> Donations { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? StatusFilter { get; set; }
        public string? TypeFilter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class DonationStatsViewModel
    {
        public int TotalDonations { get; set; }
        public int CompletedDonations { get; set; }
        public int PendingDonations { get; set; }
        public int FailedDonations { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public int MonthlyDonations { get; set; }
        public int OneTimeDonations { get; set; }
        public decimal AverageAmount { get; set; }
        public List<MonthlyStatsItem> MonthlyStats { get; set; } = new();
    }

    public class MonthlyStatsItem
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}