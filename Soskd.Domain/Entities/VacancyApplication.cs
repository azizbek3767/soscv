// Soskd.Domain/Entities/VacancyApplication.cs
using System.ComponentModel.DataAnnotations;

namespace Soskd.Domain.Entities
{
    public class VacancyApplication
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
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Application Details
        [Required]
        public string CoverLetter { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ResumeUrl { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ResumeFileName { get; set; } = string.Empty;

        // Vacancy Reference
        [Required]
        public int VacancyId { get; set; }

        [Required]
        [MaxLength(500)]
        public string VacancyTitle { get; set; } = string.Empty;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public virtual Vacancy? Vacancy { get; set; }
    }
}