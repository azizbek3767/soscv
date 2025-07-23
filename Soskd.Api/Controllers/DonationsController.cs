// Soskd.Api/Controllers/DonationsController.cs - Corrected for UPay Billing API
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;
using Soskd.Infrastructure.Services;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUpayService _upayService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DonationsController> _logger;

        public DonationsController(
            ApplicationDbContext context,
            IUpayService upayService,
            IConfiguration configuration,
            ILogger<DonationsController> logger)
        {
            _context = context;
            _upayService = upayService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Initiate donation and process UPay prepayment
        /// </summary>
        [HttpPost("initiate")]
        public async Task<ActionResult<DonationResponseDto>> InitiateDonation([FromBody] DonationRequestDto request)
        {
            try
            {
                // Validate request
                if (request.Amount < 35000)
                {
                    return BadRequest(new { message = "Minimum donation amount is 35,000 UZS" });
                }

                if (string.IsNullOrWhiteSpace(request.DonorPhone))
                {
                    return BadRequest(new { message = "Phone number is required for UPay payments" });
                }

                // Generate unique order ID
                var orderId = GenerateOrderId();

                // Get client info
                var ipAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Create donation record
                var donation = new Donation
                {
                    DonorName = request.DonorName.Trim(),
                    DonorEmail = request.DonorEmail.Trim(),
                    DonorPhone = request.DonorPhone?.Trim(),
                    Amount = request.Amount,
                    Type = request.Type,
                    Status = DonationStatus.Pending,
                    OrderId = orderId,
                    PaymentMethod = "upay",
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    IsRecurring = request.Type == DonationType.Monthly,
                    NextPaymentDate = request.Type == DonationType.Monthly
                        ? DateTime.UtcNow.AddMonths(1)
                        : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                var response = new DonationResponseDto
                {
                    OrderId = orderId,
                    Status = "initiated"
                };

                _logger.LogInformation("Donation initiated: OrderId={OrderId}, Amount={Amount}, Type={Type}",
                    orderId, request.Amount, request.Type);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating donation: {Message}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while initiating the donation" });
            }
        }

        /// <summary>
        /// Process UPay prepayment (register card and trigger SMS)
        /// </summary>
        [HttpPost("prepayment")]
        public async Task<ActionResult<UpayPrepaymentResponseDto>> ProcessPrepayment([FromBody] UpayPrepaymentRequestDto request)
        {
            try
            {
                // Find donation
                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.OrderId == request.OrderId && d.Status == DonationStatus.Pending);

                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found or already processed" });
                }

                // Prepare UPay prepayment request
                var upayRequest = new UpayPrepaymentRequest
                {
                    ServiceId = _configuration["UPay:ServiceId"] ?? "8",
                    OrderId = donation.OrderId,
                    Amount = donation.Amount,
                    Phone = donation.DonorPhone!,
                    CardNumber = request.CardNumber.Replace(" ", ""),
                    ExpireDate = request.ExpiryDate,
                    PersonalAccount = donation.DonorEmail,
                    ReturnUrl = _configuration["UPay:ReturnUrl"] ?? ""
                };

                // Call UPay prepayment API
                var upayResponse = await _upayService.PrepaymentAsync(upayRequest);

                if (!upayResponse.Success)
                {
                    _logger.LogWarning("UPay prepayment failed: OrderId={OrderId}, Error={ErrorMessage}",
                        request.OrderId, upayResponse.ErrorMessage);

                    return BadRequest(new { message = upayResponse.ErrorMessage ?? "Prepayment failed" });
                }

                // Update donation with transaction ID
                donation.UpayTransactionId = upayResponse.TransactionId;
                donation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var response = new UpayPrepaymentResponseDto
                {
                    Success = true,
                    TransactionId = upayResponse.TransactionId,
                    Status = upayResponse.Status,
                    RequiresConfirmation = upayResponse.RequiresConfirmation,
                    Message = upayResponse.RequiresConfirmation ?
                        "SMS code sent to your phone number" :
                        "Payment processed successfully"
                };

                _logger.LogInformation("UPay prepayment successful: OrderId={OrderId}, TransactionId={TransactionId}",
                    request.OrderId, upayResponse.TransactionId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing prepayment: {Message}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while processing prepayment" });
            }
        }

        /// <summary>
        /// Confirm UPay payment with SMS code
        /// </summary>
        [HttpPost("confirm-payment")]
        public async Task<ActionResult<UpayConfirmPaymentResponseDto>> ConfirmPayment([FromBody] UpayConfirmPaymentRequestDto request)
        {
            try
            {
                // Find donation
                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.OrderId == request.OrderId && d.Status == DonationStatus.Pending);

                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found or already processed" });
                }

                if (string.IsNullOrEmpty(donation.UpayTransactionId))
                {
                    return BadRequest(new { message = "Prepayment not completed. Please complete prepayment first." });
                }

                // Prepare UPay confirm payment request
                var upayRequest = new UpayConfirmPaymentRequest
                {
                    ServiceId = _configuration["UPay:ServiceId"] ?? "8",
                    TransactionId = donation.UpayTransactionId,
                    SmsCode = request.SmsCode
                };

                // Call UPay confirm payment API
                var upayResponse = await _upayService.ConfirmPaymentAsync(upayRequest);

                if (!upayResponse.Success)
                {
                    _logger.LogWarning("UPay payment confirmation failed: OrderId={OrderId}, Error={ErrorMessage}",
                        request.OrderId, upayResponse.ErrorMessage);

                    // Check if it's an SMS code error
                    if (upayResponse.ErrorCode?.Contains("SMS") == true ||
                        upayResponse.ErrorMessage?.ToLower().Contains("sms") == true ||
                        upayResponse.ErrorMessage?.ToLower().Contains("code") == true)
                    {
                        return BadRequest(new { message = "Invalid SMS code. Please try again." });
                    }

                    return BadRequest(new { message = upayResponse.ErrorMessage ?? "Payment confirmation failed" });
                }

                // Update donation status
                donation.Status = DonationStatus.Completed;
                donation.ProcessedAmount = upayResponse.ProcessedAmount > 0 ? upayResponse.ProcessedAmount : donation.Amount;
                donation.PaymentCompletedAt = DateTime.UtcNow;
                donation.UpdatedAt = DateTime.UtcNow;

                // For monthly donations, schedule next payment
                if (donation.Type == DonationType.Monthly && donation.IsRecurring)
                {
                    await ScheduleNextMonthlyPayment(donation);
                }

                await _context.SaveChangesAsync();

                var response = new UpayConfirmPaymentResponseDto
                {
                    Success = true,
                    TransactionId = upayResponse.TransactionId,
                    Status = upayResponse.Status,
                    ProcessedAmount = donation.ProcessedAmount ?? donation.Amount,
                    Message = "Payment completed successfully"
                };

                _logger.LogInformation("Payment confirmed successfully: OrderId={OrderId}, TransactionId={TransactionId}",
                    request.OrderId, upayResponse.TransactionId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment: {Message}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while confirming payment" });
            }
        }

        /// <summary>
        /// Get donation status
        /// </summary>
        [HttpGet("status/{orderId}")]
        public async Task<ActionResult<DonationStatusDto>> GetDonationStatus(string orderId)
        {
            try
            {
                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.OrderId == orderId);

                if (donation == null)
                {
                    return NotFound(new { message = "Donation not found" });
                }

                var status = new DonationStatusDto
                {
                    Id = donation.Id,
                    OrderId = donation.OrderId,
                    DonorName = donation.DonorName,
                    DonorEmail = donation.DonorEmail,
                    Amount = donation.Amount,
                    Type = donation.Type.ToString(),
                    Status = donation.Status.ToString(),
                    UpayTransactionId = donation.UpayTransactionId,
                    PaymentMethod = donation.PaymentMethod,
                    PaymentCompletedAt = donation.PaymentCompletedAt,
                    CreatedAt = donation.CreatedAt
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donation status: {Message}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while getting donation status" });
            }
        }

        /// <summary>
        /// UPay callback handler
        /// </summary>
        [HttpPost("upay-callback")]
        public async Task<IActionResult> HandleUpayCallback([FromForm] UpayCallbackDto callback)
        {
            try
            {
                _logger.LogInformation("UPay callback received: OrderId={OrderId}, TransactionId={TransactionId}, Status={Status}",
                    callback.OrderId, callback.TransactionId, callback.Status);

                // Verify callback
                var callbackRequest = new UpayCallbackRequest
                {
                    ServiceId = callback.ServiceId,
                    OrderId = callback.OrderId,
                    TransactionId = callback.TransactionId,
                    Status = callback.Status,
                    Amount = callback.Amount,
                    TransactionTime = callback.TransactionTime,
                    PersonalAccount = callback.PersonalAccount,
                    AccessToken = callback.AccessToken
                };

                if (!_upayService.VerifyCallback(callbackRequest))
                {
                    _logger.LogWarning("Invalid UPay callback: OrderId={OrderId}", callback.OrderId);
                    return BadRequest("Invalid callback");
                }

                // Find donation
                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.OrderId == callback.OrderId);

                if (donation == null)
                {
                    _logger.LogWarning("Donation not found for callback: OrderId={OrderId}", callback.OrderId);
                    return NotFound("Donation not found");
                }

                // Update donation status based on callback
                if (callback.Status.ToUpper() == "SUCCESS")
                {
                    donation.Status = DonationStatus.Completed;
                    donation.ProcessedAmount = callback.Amount;
                    donation.PaymentCompletedAt = callback.TransactionTime;
                    donation.UpayTransactionId = callback.TransactionId;

                    // For monthly donations, schedule next payment
                    if (donation.Type == DonationType.Monthly && donation.IsRecurring)
                    {
                        await ScheduleNextMonthlyPayment(donation);
                    }
                }
                else
                {
                    donation.Status = DonationStatus.Failed;
                    donation.FailureReason = $"Payment failed via callback. Status: {callback.Status}";
                }

                donation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("UPay callback processed: OrderId={OrderId}, Status={Status}",
                    callback.OrderId, donation.Status);

                return Ok("Callback processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UPay callback: {Message}", ex.Message);
                return StatusCode(500, "Callback processing failed");
            }
        }

        /// <summary>
        /// Get recent donations for public display
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<List<DonationStatusDto>>> GetRecentDonations([FromQuery] int count = 10)
        {
            try
            {
                count = Math.Min(50, Math.Max(1, count));

                var donations = await _context.Donations
                    .Where(d => d.Status == DonationStatus.Completed)
                    .OrderByDescending(d => d.PaymentCompletedAt)
                    .Take(count)
                    .Select(d => new DonationStatusDto
                    {
                        Id = d.Id,
                        OrderId = d.OrderId,
                        DonorName = MaskName(d.DonorName),
                        DonorEmail = MaskEmail(d.DonorEmail),
                        Amount = d.Amount,
                        Type = d.Type.ToString(),
                        Status = d.Status.ToString(),
                        PaymentCompletedAt = d.PaymentCompletedAt,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(donations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent donations: {Message}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while getting recent donations" });
            }
        }

        private string GenerateOrderId()
        {
            return $"DON_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}"[..20];
        }

        private async Task ScheduleNextMonthlyPayment(Donation parentDonation)
        {
            var nextPayment = new Donation
            {
                DonorName = parentDonation.DonorName,
                DonorEmail = parentDonation.DonorEmail,
                DonorPhone = parentDonation.DonorPhone,
                Amount = parentDonation.Amount,
                Type = DonationType.Monthly,
                Status = DonationStatus.Pending,
                OrderId = GenerateOrderId(),
                PaymentMethod = parentDonation.PaymentMethod,
                IpAddress = parentDonation.IpAddress,
                UserAgent = parentDonation.UserAgent,
                IsRecurring = true,
                ParentDonationId = parentDonation.Id,
                NextPaymentDate = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Donations.Add(nextPayment);
            parentDonation.NextPaymentDate = nextPayment.NextPaymentDate;
            parentDonation.UpdatedAt = DateTime.UtcNow;
        }

        private string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
            }

            return ipAddress ?? "Unknown";
        }

        private static string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length <= 3)
                return name;

            return name[0] + "***" + name[^1];
        }

        private static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                return email;

            var parts = email.Split('@');
            if (parts[0].Length <= 3)
                return email;

            return parts[0][..2] + "***@" + parts[1];
        }
    }
}