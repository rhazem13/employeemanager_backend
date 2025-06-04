using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Interfaces
{
    public interface IAttendanceRepository
    {
        Task AddAsync(Attendance attendance);
        Task<bool> HasCheckedInTodayAsync(int employeeId, DateTime date);
        Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Attendance>> GetAllAsync(DateTime date);
    }
}
