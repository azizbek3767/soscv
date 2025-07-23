using Soskd.Domain.Enums;

namespace Soskd.Api.DTOs
{
    public class DonationRequestDto
    {
        public string DonorName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public string? DonorPhone { get; set; }
        public decimal Amount { get; set; }
        public DonationType Type { get; set; }
    }

    public class DonationResponseDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentFormHtml { get; set; } = string.Empty;
        public string? RedirectUrl { get; set; }
    }

    public class DonationStatusDto
    {
        public int Id { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? UpayTransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentCompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DonationListDto
    {
        public List<DonationStatusDto> Donations { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class CardRegistrationRequestDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
    }

    public class CardRegistrationResponseDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class PaymentConfirmationRequestDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }

    public class PaymentConfirmationResponseDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class UpayPrepaymentRequestDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
    }

    public class UpayPrepaymentResponseDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool RequiresConfirmation { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class UpayConfirmPaymentRequestDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string SmsCode { get; set; } = string.Empty;
    }

    public class UpayConfirmPaymentResponseDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal ProcessedAmount { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class UpayCallbackDto
    {
        public string ServiceId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public string PersonalAccount { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}