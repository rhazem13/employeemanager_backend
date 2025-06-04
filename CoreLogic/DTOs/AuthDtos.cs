using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;
using System.ComponentModel.DataAnnotations;


namespace CoreLogic.DTOs
{
    public class AuthDtos
    {
        public class RegisterDto
        {
            [Required]
            [StringLength(50)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [StringLength(50)]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [Phone]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required]
            [StringLength(20)]
            public string NationalId { get; set; } = string.Empty;

            [Required]
            [Range(18, 100)]
            public int Age { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;

            [Required]
            public Role Role { get; set; } // Admin or Employee
        }
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class TokenResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public int EmployeeId { get; set; }
        }
    }
   
}
