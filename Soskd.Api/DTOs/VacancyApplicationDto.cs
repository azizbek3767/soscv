// Soskd.Api/DTOs/VacancyApplicationDto.cs
using System.ComponentModel.DataAnnotations;

namespace Soskd.Api.DTOs
{
    public class VacancyApplicationDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        public string ResumeFileName { get; set; } = string.Empty;
        public int VacancyId { get; set; }
        public string VacancyTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateVacancyApplicationDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Cover letter is required")]
        [StringLength(5000, ErrorMessage = "Cover letter cannot exceed 5000 characters")]
        public string CoverLetter { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vacancy ID is required")]
        public int VacancyId { get; set; }

        [Required(ErrorMessage = "Vacancy title is required")]
        [StringLength(500, ErrorMessage = "Vacancy title cannot exceed 500 characters")]
        public string VacancyTitle { get; set; } = string.Empty;
    }

    public class VacancyApplicationListDto
    {
        public List<VacancyApplicationDto> Applications { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}