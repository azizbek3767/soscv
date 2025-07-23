namespace Soskd.Api.DTOs
{
    public class VacancyDto
    {
        public int Id { get; set; }
        // Titles
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions
        public string DescriptionUz { get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        // Dates
        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Status information
        public bool IsPublished => PublishedDate.HasValue && PublishedDate.Value <= DateTime.UtcNow;
        public bool IsExpired => Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
        public bool IsActive => IsPublished && !IsExpired;
        public string Status
        {
            get
            {
                if (!IsPublished) return "Draft";
                if (IsExpired) return "Expired";
                return "Active";
            }
        }
    }
    public class VacancyListDto
    {
        public List<VacancyItemDto> Vacancies { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    public class VacancyItemDto
    {
        public int Id { get; set; }
        // Titles
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions (truncated for list view)
        public string DescriptionUz { get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        // Dates
        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        // Status information
        public bool IsPublished => PublishedDate.HasValue && PublishedDate.Value <= DateTime.UtcNow;
        public bool IsExpired => Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
        public bool IsActive => IsPublished && !IsExpired;
        public string Status
        {
            get
            {
                if (!IsPublished) return "Draft";
                if (IsExpired) return "Expired";
                return "Active";
            }
        }
        // Time-related helper properties
        public int? DaysUntilDeadline => Deadline.HasValue && !IsExpired
            ? (int?)(Deadline.Value - DateTime.UtcNow).Days
            : null;
    }
    public class VacancySummaryDto
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

