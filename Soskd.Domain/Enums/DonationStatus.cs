namespace Soskd.Domain.Enums
{
    public enum DonationStatus
    {
        Pending = 1,      // Payment initiated but not completed
        Completed = 2,    // Payment successful
        Failed = 3,       // Payment failed
        Cancelled = 4,    // Payment cancelled by user
        Refunded = 5      // Payment refunded
    }
}