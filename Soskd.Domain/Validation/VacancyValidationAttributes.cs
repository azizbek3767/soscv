// Soskd.Domain/Validation/VacancyValidationAttributes.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Validation;

namespace Soskd.Domain.Validation
{
    /// <summary>
    /// Validates that the deadline is after the published date
    /// </summary>
    public class DeadlineAfterPublishedDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var deadline = value as DateTime?;

            if (!deadline.HasValue)
                return ValidationResult.Success; // Deadline is optional

            var publishedDateProperty = validationContext.ObjectType.GetProperty("PublishedDate");
            if (publishedDateProperty == null)
                return ValidationResult.Success;

            var publishedDate = publishedDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (!publishedDate.HasValue)
                return ValidationResult.Success; // Published date is optional

            if (deadline.Value <= publishedDate.Value)
            {
                return new ValidationResult("Deadline must be after the published date.");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates that the published date is not in the past (with some tolerance)
    /// </summary>
    public class NotInPastAttribute : ValidationAttribute
    {
        private readonly int _toleranceMinutes;

        public NotInPastAttribute(int toleranceMinutes = 5)
        {
            _toleranceMinutes = toleranceMinutes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = value as DateTime?;

            if (!date.HasValue)
                return ValidationResult.Success; // Optional date

            var minAllowedDate = DateTime.Now.AddMinutes(-_toleranceMinutes);

            if (date.Value < minAllowedDate)
            {
                return new ValidationResult($"Date cannot be more than {_toleranceMinutes} minutes in the past.");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates that the deadline is not too far in the future
    /// </summary>
    public class ReasonableDeadlineAttribute : ValidationAttribute
    {
        private readonly int _maxDaysInFuture;

        public ReasonableDeadlineAttribute(int maxDaysInFuture = 365)
        {
            _maxDaysInFuture = maxDaysInFuture;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var deadline = value as DateTime?;

            if (!deadline.HasValue)
                return ValidationResult.Success; // Optional deadline

            var maxAllowedDate = DateTime.Now.AddDays(_maxDaysInFuture);

            if (deadline.Value > maxAllowedDate)
            {
                return new ValidationResult($"Deadline cannot be more than {_maxDaysInFuture} days in the future.");
            }

            return ValidationResult.Success;
        }
    }
}

