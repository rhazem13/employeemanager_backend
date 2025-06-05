using CoreLogic.DTOs;
using CoreLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public DashboardService(IEmployeeRepository employeeRepository, IAttendanceRepository attendanceRepository)
        {
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            // Get total employee count
            var totalEmployees = await _employeeRepository.CountAsync(null);

            // Get today's date in EEST
            var today = DateTime.Now.Date;

            // Get count of employees who checked in today
            var presentToday = await _attendanceRepository.CountCheckedInTodayAsync(today);

            // Calculate non-present today
            var nonPresentToday = totalEmployees - presentToday;

            return new DashboardStatsDto
            {
                TotalEmployees = totalEmployees,
                PresentToday = presentToday,
                NonPresentToday = nonPresentToday
            };
        }
    }
}
