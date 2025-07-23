// Soskd.Api/Controllers/ContactController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Domain.Entities;
using Soskd.Infrastructure.Data;
using System.Net;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactController> _logger;

        public ContactController(ApplicationDbContext context, ILogger<ContactController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Submit a contact form application
        /// </summary>
        /// <param name="submission">Contact form data</param>
        /// <returns>Response indicating success or failure</returns>
        [HttpPost("submit")]
        public async Task<ActionResult<ContactSubmissionResponseDto>> SubmitContact([FromBody] ContactSubmissionDto submission)
        {
            try
            {
                _logger.LogInformation("Received contact form submission from {Email}", submission.Email);

                // Additional server-side validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Contact form validation failed: {Errors}", string.Join(", ", errors));

                    return BadRequest(new ContactSubmissionResponseDto
                    {
                        Success = false,
                        Message = $"Validation failed: {string.Join(", ", errors)}"
                    });
                }

                // Get client IP address
                var clientIpAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Check for potential spam (basic rate limiting by IP)
                var recentSubmissions = await _context.ContactApplications
                    .Where(c => c.IpAddress == clientIpAddress &&
                               c.CreatedAt > DateTime.UtcNow.AddMinutes(-5))
                    .CountAsync();

                if (recentSubmissions >= 3)
                {
                    _logger.LogWarning("Rate limit exceeded for IP {IpAddress}", clientIpAddress);
                    return BadRequest(new ContactSubmissionResponseDto
                    {
                        Success = false,
                        Message = "Too many submissions. Please wait before submitting again."
                    });
                }

                // Create the contact application
                var contactApplication = new ContactApplication
                {
                    FullName = submission.FullName.Trim(),
                    Email = submission.Email.Trim().ToLower(),
                    Phone = submission.Phone?.Trim(),
                    City = submission.City.Trim(),
                    Message = submission.Message.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = clientIpAddress,
                    UserAgent = userAgent.Length > 500 ? userAgent.Substring(0, 500) : userAgent
                };

                _context.ContactApplications.Add(contactApplication);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Contact application saved successfully with ID {Id} from {Email}",
                    contactApplication.Id, contactApplication.Email);

                return Ok(new ContactSubmissionResponseDto
                {
                    Success = true,
                    Message = "Your message has been sent successfully! We will get back to you soon.",
                    ApplicationId = contactApplication.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission from {Email}", submission.Email);

                return StatusCode(500, new ContactSubmissionResponseDto
                {
                    Success = false,
                    Message = "An error occurred while processing your message. Please try again later."
                });
            }
        }

        /// <summary>
        /// Get contact form statistics (for public display)
        /// </summary>
        /// <returns>Basic statistics about contact submissions</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetContactStats()
        {
            try
            {
                var totalApplications = await _context.ContactApplications.CountAsync();
                var thisMonthApplications = await _context.ContactApplications
                    .Where(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                    .CountAsync();

                return Ok(new
                {
                    totalApplications,
                    thisMonthApplications,
                    message = "Thank you for your interest in SOS Children's Village!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact statistics");
                return StatusCode(500, new { message = "Unable to retrieve statistics" });
            }
        }

        /// <summary>
        /// Helper method to get client IP address
        /// </summary>
        /// <returns>Client IP address</returns>
        private string GetClientIpAddress()
        {
            try
            {
                // Try to get IP from X-Forwarded-For header (common in load balancers/proxies)
                var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    // X-Forwarded-For can contain multiple IPs, take the first one
                    var ip = forwardedFor.Split(',')[0].Trim();
                    if (IPAddress.TryParse(ip, out _))
                        return ip;
                }

                // Try X-Real-IP header
                var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp) && IPAddress.TryParse(realIp, out _))
                    return realIp;

                // Fall back to connection remote IP
                var connectionIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(connectionIp))
                    return connectionIp;

                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}