using CoreLogic.DTOs;
using FluentValidation;

namespace Web.Validators
{
    public class CheckInDtoValidator : AbstractValidator<CheckInDto>
    {
        public CheckInDtoValidator()
        {
            RuleFor(x => x.CheckInTime)
                .Must(time => time == null || time <= DateTime.UtcNow.AddMinutes(5))
                .WithMessage("Check-in time cannot be in the future.");
        }
    }
}
