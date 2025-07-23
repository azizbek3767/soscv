// Soskd.Domain/Entities/Application.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;

namespace Soskd.Domain.Entities
{
    public class Application
    {
        public int Id { get; set; }

        // Personal Information
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Application Details
        [Required]
        public string CoverLetter { get; set; } = string.Empty;

        // Position Information
        [Required]
        [MaxLength(500)]
        public string Position { get; set; } = string.Empty;

        [Required]
        public int VacancyId { get; set; }

        // File Information
        [Required]
        [MaxLength(500)]
        public string ResumeFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ResumeFilePath { get; set; } = string.Empty;

        [Required]
        public long ResumeFileSize { get; set; }

        [Required]
        [MaxLength(50)]
        public string ResumeContentType { get; set; } = string.Empty;

        // Consent
        [Required]
        public bool DataProcessingConsent { get; set; }

        [Required]
        public bool TermsAndConditionsConsent { get; set; }

        // Application Status
        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

        // reCAPTCHA and Security
        [MaxLength(1000)]
        public string? RecaptchaToken { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        // Audit Fields
        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        [MaxLength(100)]
        public string? ReviewedBy { get; set; }

        public string? ReviewNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Vacancy? Vacancy { get; set; }

        // Helper Properties
        public string FullName => $"{FirstName} {LastName}";

        public string FormattedFileSize
        {
            get
            {
                if (ResumeFileSize < 1024) return $"{ResumeFileSize} B";
                if (ResumeFileSize < 1024 * 1024) return $"{ResumeFileSize / 1024.0:F1} KB";
                if (ResumeFileSize < 1024 * 1024 * 1024) return $"{ResumeFileSize / (1024.0 * 1024.0):F1} MB";
                return $"{ResumeFileSize / (1024.0 * 1024.0 * 1024.0):F1} GB";
            }
        }

        public bool IsUnderReview => Status == ApplicationStatus.UnderReview;
        public bool IsAccepted => Status == ApplicationStatus.Accepted;
        public bool IsRejected => Status == ApplicationStatus.Rejected;
        public bool IsPending => Status == ApplicationStatus.Submitted;
    }
}