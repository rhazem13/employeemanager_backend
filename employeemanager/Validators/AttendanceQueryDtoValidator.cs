using CoreLogic.DTOs;
using FluentValidation;

namespace Web.Validators
{
    public class AttendanceQueryDtoValidator : AbstractValidator<AttendanceQueryDto>
    {
        public AttendanceQueryDtoValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Start date must be before or equal to end date.");

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .When(x => x.EmployeeId.HasValue)
                .WithMessage("Employee ID must be a positive integer.");
        }
    }
}
