using CoreLogic.DTOs;
using CoreLogic.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<AttendanceService> _logger; // Added for logging

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IEmployeeRepository employeeRepository,
            ILogger<AttendanceService> logger)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task CheckInAsync(int employeeId, CheckInDto checkInDto)
        {
            // Verify employee exists
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            // Use provided time or current time, ensuring EEST (UTC+3)
            var checkInTime = checkInDto.CheckInTime?.ToUniversalTime() ?? DateTime.UtcNow;
            var eestTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); // EEST
            var checkInTimeEest = TimeZoneInfo.ConvertTimeFromUtc(checkInTime, eestTimeZone);

            _logger.LogInformation($"Check-in time (input): {checkInDto.CheckInTime}, Converted to EEST: {checkInTimeEest}");

            // Check if check-in is within the allowed time window (7:30 AM - 9:00 AM EEST)
            var timeOfDay = checkInTimeEest.TimeOfDay;
            var startTime = new TimeSpan(7, 30, 0); // 7:30 AM
            var endTime = new TimeSpan(9, 0, 0);   // 9:00 AM
            if (timeOfDay < startTime || timeOfDay > endTime)
                throw new Exception($"Check-in is only allowed between 7:30 AM and 9:00 AM EEST. Current EEST time: {checkInTimeEest:HH:mm:ss}");

            // Check for duplicate check-in
            var hasCheckedIn = await _attendanceRepository.HasCheckedInTodayAsync(employeeId, checkInTimeEest);
            if (hasCheckedIn)
                throw new Exception("You have already checked in today.");

            // Create and save attendance record
            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                CheckInTime = checkInTimeEest
            };

            await _attendanceRepository.AddAsync(attendance);
        }

        public async Task<CheckInHistoryDto> GetCheckInHistoryAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Verify employee exists
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            // Get check-in records
            var checkIns = await _attendanceRepository.GetByEmployeeIdAsync(employeeId, startDate, endDate);
            var checkInRecords = checkIns.Select(a => new CheckInRecord
            {
                Id = a.Id,
                CheckInTime = a.CheckInTime
            }).ToList();

            // Calculate weekly summaries
            var weeklySummaries = new List<WeeklySummary>();
            if (checkInRecords.Any())
            {
                // Group check-ins by week (Monday to Sunday)
                var minDate = checkInRecords.Min(c => c.CheckInTime.Date);
                var maxDate = checkInRecords.Max(c => c.CheckInTime.Date);
                var currentWeekStart = minDate.AddDays(-(int)minDate.DayOfWeek + (int)DayOfWeek.Monday);

                while (currentWeekStart <= maxDate)
                {
                    var weekEnd = currentWeekStart.AddDays(7);
                    var weekCheckIns = checkInRecords
                        .Count(c => c.CheckInTime.Date >= currentWeekStart && c.CheckInTime.Date < weekEnd);
                    weeklySummaries.Add(new WeeklySummary
                    {
                        WeekStart = currentWeekStart,
                        CheckInCount = weekCheckIns
                    });
                    currentWeekStart = weekEnd;
                }
            }

            return new CheckInHistoryDto
            {
                CheckIns = checkInRecords,
                WeeklySummaries = weeklySummaries
            };
        }
    }
}
