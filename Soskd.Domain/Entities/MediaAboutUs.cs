// Soskd.Domain/Entities/MediaAboutUs.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;
namespace Soskd.Domain.Entities
{
    public class MediaAboutUs
    {
        public int Id { get; set; }
        // Titles in three languages
        [Required]
        [MaxLength(500)]
        public string TitleUz { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions in three languages
        [Required]
        public string DescriptionUz { get; set; } = string.Empty;
        [Required]
        public string DescriptionRu { get; set; } = string.Empty;
        [Required]
        public string DescriptionEn { get; set; } = string.Empty;
        // Photo URL (flexible dimensions, recommended 400x266)
        [Required]
        [MaxLength(500)]
        public string PhotoUrl { get; set; } = string.Empty;
        // Link to external source
        [Required]
        [MaxLength(1000)]
        public string SourceLink { get; set; } = string.Empty;
        // Publication status
        [Required]
        public MediaStatus Status { get; set; } = MediaStatus.Published;
        // Dates
        [Required]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}