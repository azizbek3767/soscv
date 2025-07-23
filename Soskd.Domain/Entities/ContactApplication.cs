// Soskd.Domain/Entities/ContactApplication.cs
using System.ComponentModel.DataAnnotations;

namespace Soskd.Domain.Entities
{
    public class ContactApplication
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        // Audit Fields
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // IP Address for security/tracking
        [MaxLength(45)] // IPv6 can be up to 45 characters
        public string? IpAddress { get; set; }

        // User Agent for tracking
        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }
}