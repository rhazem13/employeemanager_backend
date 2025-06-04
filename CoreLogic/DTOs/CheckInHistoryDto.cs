using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.DTOs
{
    public class CheckInHistoryDto
    {
        public List<CheckInRecord> CheckIns { get; set; } = new List<CheckInRecord>();
        public List<WeeklySummary> WeeklySummaries { get; set; } = new List<WeeklySummary>();
    }

    public class CheckInRecord
    {
        public int Id { get; set; }
        public DateTime CheckInTime { get; set; }
    }

    public class WeeklySummary
    {
        public DateTime WeekStart { get; set; }
        public int CheckInCount { get; set; }
    }
}
