// Soskd.Domain/Enums/ApplicationStatus.cs
namespace Soskd.Domain.Enums
{
    public enum ApplicationStatus
    {
        Submitted = 1,
        UnderReview = 2,
        Accepted = 3,
        Rejected = 4,
        Withdrawn = 5,
        OnHold = 6
    }

    public static class ApplicationStatusExtensions
    {
        public static string GetDisplayName(this ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Submitted => "Submitted",
                ApplicationStatus.UnderReview => "Under Review",
                ApplicationStatus.Accepted => "Accepted",
                ApplicationStatus.Rejected => "Rejected",
                ApplicationStatus.Withdrawn => "Withdrawn",
                ApplicationStatus.OnHold => "On Hold",
                _ => status.ToString()
            };
        }

        public static string GetColor(this ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Submitted => "blue",
                ApplicationStatus.UnderReview => "yellow",
                ApplicationStatus.Accepted => "green",
                ApplicationStatus.Rejected => "red",
                ApplicationStatus.Withdrawn => "gray",
                ApplicationStatus.OnHold => "orange",
                _ => "gray"
            };
        }
    }
}