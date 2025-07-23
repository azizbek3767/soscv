// Soskd.Infrastructure/Services/IUpayService.cs - Corrected Implementation
using Soskd.Domain.Entities;

namespace Soskd.Infrastructure.Services
{
    public interface IUpayService
    {
        Task<UpayPrepaymentResponse> PrepaymentAsync(UpayPrepaymentRequest request);
        Task<UpayConfirmPaymentResponse> ConfirmPaymentAsync(UpayConfirmPaymentRequest request);
        Task<UpayCheckPaymentResponse> CheckPaymentAsync(string orderId);
        bool VerifyCallback(UpayCallbackRequest callback);
    }

    // DTOs based on actual UPay API structure
    public class UpayPrepaymentRequest
    {
        public string ServiceId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpireDate { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string PersonalAccount { get; set; } = string.Empty; // Usually email or phone
    }

    public class UpayPrepaymentResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool RequiresConfirmation { get; set; }
    }

    public class UpayConfirmPaymentRequest
    {
        public string ServiceId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string SmsCode { get; set; } = string.Empty;
    }

    public class UpayConfirmPaymentResponse
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public decimal ProcessedAmount { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }

    public class UpayCheckPaymentResponse
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class UpayCallbackRequest
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
