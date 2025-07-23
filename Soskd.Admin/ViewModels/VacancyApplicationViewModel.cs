// Soskd.Admin/ViewModels/VacancyApplicationViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Soskd.Admin.ViewModels
{
    public class VacancyApplicationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "Cover Letter")]
        public string CoverLetter { get; set; } = string.Empty;

        [Display(Name = "Resume URL")]
        public string ResumeUrl { get; set; } = string.Empty;

        [Display(Name = "Resume File Name")]
        public string ResumeFileName { get; set; } = string.Empty;

        [Display(Name = "Vacancy ID")]
        public int VacancyId { get; set; }

        [Display(Name = "Vacancy Title")]
        public string VacancyTitle { get; set; } = string.Empty;

        [Display(Name = "Applied Date")]
        public DateTime CreatedAt { get; set; }

        // Helper properties for display
        public string FullName => $"{FirstName} {LastName}";
        public string ShortCoverLetter => CoverLetter.Length > 150 ? CoverLetter.Substring(0, 150) + "..." : CoverLetter;
    }

    public class VacancyApplicationListViewModel
    {
        public List<VacancyApplicationItemViewModel> Applications { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int? SelectedVacancyId { get; set; }
        public string? SelectedVacancyTitle { get; set; }
        public List<VacancyFilterOption> VacancyFilters { get; set; } = new();
    }

    public class VacancyApplicationItemViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string ShortCoverLetter { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        public string ResumeFileName { get; set; } = string.Empty;
        public int VacancyId { get; set; }
        public string VacancyTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Helper properties
        public string FullName => $"{FirstName} {LastName}";
    }

    public class VacancyFilterOption
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }
}