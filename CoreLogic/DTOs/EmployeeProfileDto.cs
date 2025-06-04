using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class EmployeeProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Signature { get; set; } // Base64 string
        public string Email { get; set; } = string.Empty;
        public List<WeeklySummary> WeeklyAttendanceHistory { get; set; } = new List<WeeklySummary>();
    }

}
