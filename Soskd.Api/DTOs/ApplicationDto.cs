// Soskd.Api/DTOs/ApplicationDto.cs
using System.ComponentModel.DataAnnotations;

namespace Soskd.Api.DTOs
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int VacancyId { get; set; }
        public string ResumeFileName { get; set; } = string.Empty;
        public long ResumeFileSize { get; set; }
        public string FormattedFileSize { get; set; } = string.Empty;
        public string ResumeContentType { get; set; } = string.Empty;
        public bool DataProcessingConsent { get; set; }
        public bool TermsAndConditionsConsent { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Vacancy Information
        public VacancyBasicDto? Vacancy { get; set; }
    }

    public class ApplicationListDto
    {
        public List<ApplicationItemDto> Applications { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class ApplicationItemDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Position { get; set; } = string.Empty;
        public int VacancyId { get; set; }
        public string ResumeFileName { get; set; } = string.Empty;
        public string FormattedFileSize { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }

        // Truncated cover letter for list view
        public string CoverLetterSummary { get; set; } = string.Empty;
    }

    public class VacancyBasicDto
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ApplicationSubmissionRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Cover letter is required")]
        [StringLength(5000, MinimumLength = 50, ErrorMessage = "Cover letter must be between 50 and 5000 characters")]
        public string CoverLetter { get; set; } = string.Empty;

        [Required(ErrorMessage = "Position is required")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Position ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid position ID is required")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "Data processing consent is required")]
        public bool DataConsent { get; set; }

        [Required(ErrorMessage = "Terms and conditions consent is required")]
        public bool TermsConsent { get; set; }

        [Required(ErrorMessage = "reCAPTCHA verification is required")]
        public string RecaptchaToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resume file is required")]
        public IFormFile Resume { get; set; } = null!;
    }

    public class ApplicationSubmissionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ApplicationId { get; set; }
        public int? ApplicationNumber { get; set; }
        public DateTime? SubmissionTime { get; set; }
        public string? ReferenceNumber { get; set; }
    }

    public class ApplicationStatusUpdateRequest
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? ReviewNotes { get; set; }

        [Required]
        public string ReviewedBy { get; set; } = string.Empty;
    }

    public class ApplicationStatusUpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ApplicationItemDto? Application { get; set; }
    }

    public class ApplicationFileDownloadRequest
    {
        [Required]
        public int ApplicationId { get; set; }

        public string? AccessToken { get; set; }
    }

    public class ApplicationStatisticsDto
    {
        public int TotalApplications { get; set; }
        public int SubmittedApplications { get; set; }
        public int UnderReviewApplications { get; set; }
        public int AcceptedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int WithdrawnApplications { get; set; }
        public int OnHoldApplications { get; set; }
        public int TodayApplications { get; set; }
        public int ThisWeekApplications { get; set; }
        public int ThisMonthApplications { get; set; }
        public double AcceptanceRate { get; set; }
        public double AverageProcessingDays { get; set; }
        public List<ApplicationStatusBreakdown> StatusBreakdown { get; set; } = new();
        public List<VacancyApplicationCount> TopVacancies { get; set; } = new();
    }

    public class ApplicationStatusBreakdown
    {
        public string Status { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class VacancyApplicationCount
    {
        public int VacancyId { get; set; }
        public string VacancyTitle { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
        public DateTime? LastApplication { get; set; }
    }

    public class ApplicationSearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public int? VacancyId { get; set; }
        public DateTime? SubmittedFrom { get; set; }
        public DateTime? SubmittedTo { get; set; }
        public string? SortBy { get; set; } = "SubmittedAt";
        public string? SortOrder { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}