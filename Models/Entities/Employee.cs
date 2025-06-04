using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

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

        public string? Signature { get; set; } // Base64 string for signature image

        [Required]
        public Role Role { get; set; } // Admin or Employee

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // For authentication

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Hashed password for authentication

        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
