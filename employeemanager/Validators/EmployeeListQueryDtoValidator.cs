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
                .Must(BeValidSortField).WithMessage("SortBy must be 'firstname', 'lastname', 'age', or null.")
                .When(x => !string.IsNullOrEmpty(x.SortBy));
        }

        private bool BeValidSortField(string? sortBy)
        {
            return sortBy != null && new[] { "firstname", "lastname", "age" }.Contains(sortBy.ToLower());
        }
    }
}
