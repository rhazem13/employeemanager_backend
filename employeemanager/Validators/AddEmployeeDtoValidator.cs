using CoreLogic.DTOs;
using FluentValidation;

namespace Web.Validators
{
    public class AddEmployeeDtoValidator : AbstractValidator<AddEmployeeDto>
    {
        public AddEmployeeDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must be a valid format (10-15 digits, optional leading +).");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Length(5, 20).WithMessage("National ID must be between 5 and 20 characters.");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 100).WithMessage("Age must be between 18 and 100.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*).");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Role must be either 'Admin' or 'Employee'.");

            RuleFor(x => x.Signature)
                .Must(BeValidBase64Image).WithMessage("Signature must be a valid base64-encoded PNG or JPEG image.")
                .When(x => !string.IsNullOrEmpty(x.Signature));
        }

        private bool BeValidBase64Image(string? base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return true; // Signature is optional

            string base64Data = base64String;
            if (base64String.StartsWith("data:image/"))
            {
                var commaIndex = base64String.IndexOf(",");
                if (commaIndex == -1)
                    return false;
                base64Data = base64String.Substring(commaIndex + 1);
            }

            try
            {
                var bytes = Convert.FromBase64String(base64Data);
                if (bytes.Length < 8)
                    return false;

                bool isPng = bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
                             bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
                bool isJpeg = bytes[0] == 0xFF && bytes[1] == 0xD8;

                return isPng || isJpeg;
            }
            catch
            {
                return false;
            }
        }
    }
}
