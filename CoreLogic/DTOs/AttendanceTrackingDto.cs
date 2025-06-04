using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class AttendanceTrackingDto
    {
        public List<DailyAttendanceRecord> DailyRecords { get; set; } = new List<DailyAttendanceRecord>();
        public List<WeeklyWorkingHours> WeeklySummaries { get; set; } = new List<WeeklyWorkingHours>();
    }

    public class DailyAttendanceRecord
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
    }

    public class WeeklyWorkingHours
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime WeekStart { get; set; }
        public int CheckInCount { get; set; }
        public double HoursWorked { get; set; } // Assuming 8 hours per check-in
    }
}
