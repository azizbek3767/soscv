// Soskd.Domain/Entities/Vacancy.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;
namespace Soskd.Domain.Entities
{
    public class Vacancy
    {
        public int Id { get; set; }
        // Uzbek Content
        [Required]
        [MaxLength(500)]
        public string TitleUz { get; set; } = string.Empty;
        [Required]
        public string DescriptionUz { get; set; } = string.Empty;
        // Russian Content
        [Required]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;
        [Required]
        public string DescriptionRu { get; set; } = string.Empty;
        // English Content
        [Required]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;
        [Required]
        public string DescriptionEn { get; set; } = string.Empty;
        // Status field (manageable from admin)
        [Required]
        public VacancyStatus Status { get; set; } = VacancyStatus.Published;
        // Optional dates
        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        // Audit fields
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Helper properties
        public bool IsPublished => Status == VacancyStatus.Published && (!PublishedDate.HasValue || PublishedDate.Value <= DateTime.UtcNow);
        public bool IsExpired => Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
        public bool IsActive => IsPublished && !IsExpired && Status != VacancyStatus.Closed;
        public bool IsClosed => Status == VacancyStatus.Closed;
    }
}

