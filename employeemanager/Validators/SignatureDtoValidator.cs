using CoreLogic.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Web.Validators
{
    public class SignatureDtoValidator : AbstractValidator<SignatureDto>
    {
        public SignatureDtoValidator()
        {
            RuleFor(x => x.Signature)
                .NotEmpty().WithMessage("Signature is required.")
                .Must(BeValidBase64Image).WithMessage("Signature must be a valid base64-encoded PNG or JPEG image.");
        }

        private bool BeValidBase64Image(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return false;

            // Remove data URI prefix if present (e.g., "data:image/png;base64,")
            string base64Data = base64String;
            if (base64String.StartsWith("data:image/"))
            {
                var commaIndex = base64String.IndexOf(",");
                if (commaIndex == -1)
                    return false;
                base64Data = base64String.Substring(commaIndex + 1);
            }

            // Ensure base64 string is properly padded and valid
            try
            {
                var bytes = Convert.FromBase64String(base64Data);

                // Check for PNG or JPEG header
                if (bytes.Length < 8)
                    return false;

                // PNG header: 89 50 4E 47 0D 0A 1A 0A
                bool isPng = bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
                             bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;

                // JPEG header: FF D8
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
