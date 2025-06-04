using CoreLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Interfaces
{
    public interface IAttendanceService
    {
        Task CheckInAsync(int employeeId, CheckInDto checkInDto);
        Task<CheckInHistoryDto> GetCheckInHistoryAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null);
        Task<AttendanceTrackingDto> GetAttendanceTrackingAsync(AttendanceQueryDto query);

    }
}
