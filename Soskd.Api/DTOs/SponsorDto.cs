// Soskd.Api/DTOs/SponsorDto.cs
namespace Soskd.Api.DTOs
{
    public class SponsorDto
    {
        public int Id { get; set; }

        // Titles in three languages
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;

        // Photo and link
        public string PhotoUrl { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        // Display order
        public int DisplayOrder { get; set; }

        // Status
        public string Status { get; set; } = string.Empty;
    }

    public class SponsorListDto
    {
        public List<SponsorItemDto> Sponsors { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class SponsorItemDto
    {
        public int Id { get; set; }

        // Titles in three languages
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;

        // Photo and link
        public string PhotoUrl { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        // Display order
        public int DisplayOrder { get; set; }
    }
}