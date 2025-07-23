// Soskd.Domain/Entities/Sponsor.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;

namespace Soskd.Domain.Entities
{
    public class Sponsor
    {
        public int Id { get; set; }

        // Titles in three languages
        [Required]
        [MaxLength(200)]
        public string TitleUz { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TitleRu { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TitleEn { get; set; } = string.Empty;

        // Logo/Photo
        [Required]
        [MaxLength(500)]
        public string PhotoUrl { get; set; } = string.Empty;

        // Website link
        [Required]
        [MaxLength(500)]
        public string Link { get; set; } = string.Empty;

        // Status
        [Required]
        public SponsorStatus Status { get; set; } = SponsorStatus.Published;

        // Display order for sorting
        public int DisplayOrder { get; set; } = 0;

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}