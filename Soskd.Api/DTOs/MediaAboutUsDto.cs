// Soskd.Api/DTOs/MediaAboutUsDto.cs
namespace Soskd.Api.DTOs
{
    public class MediaAboutUsDto
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
        // Media and links
        public string PhotoUrl { get; set; } = string.Empty;
        public string SourceLink { get; set; } = string.Empty;
        // Status and dates
        public string Status { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
    }
    public class MediaAboutUsListDto
    {
        public List<MediaAboutUsItemDto> MediaItems { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    public class MediaAboutUsItemDto
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
        // Media and links
        public string PhotoUrl { get; set; } = string.Empty;
        public string SourceLink { get; set; } = string.Empty;
        // Dates
        public DateTime PublishDate { get; set; }
    }
}