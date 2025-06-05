using CoreLogic.DTOs;
using FluentValidation;

namespace Web.Validators
{
    public class EmployeeListQueryDtoValidator : AbstractValidator<EmployeeListQueryDto>
    {
        public EmployeeListQueryDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .Must(BeValidSortBy).WithMessage("SortBy must be one of: firstname, lastname, age.")
                .When(x => !string.IsNullOrEmpty(x.SortBy));

            RuleFor(x => x.SortDirection)
                .Must(BeValidSortDirection).WithMessage("SortDirection must be 'asc' or 'desc'.")
                .When(x => !string.IsNullOrEmpty(x.SortDirection));
        }

        private bool BeValidSortBy(string? sortBy)
        {
            if (string.IsNullOrEmpty(sortBy))
                return true;

            var validSortFields = new[] { "firstname", "lastname", "age" };
            return validSortFields.Contains(sortBy.ToLower());
        }

        private bool BeValidSortDirection(string? sortDirection)
        {
            if (string.IsNullOrEmpty(sortDirection))
                return true;

            var validDirections = new[] { "asc", "desc" };
            return validDirections.Contains(sortDirection.ToLower());
        }
    }
}
