using CoreLogic.Interfaces;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
namespace DataAccess.Repositories
{
    public  class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasCheckedInTodayAsync(int employeeId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            return await _context.Attendances
                .AnyAsync(a => a.EmployeeId == employeeId &&
                              a.CheckInTime >= startOfDay &&
                              a.CheckInTime < endOfDay);
        }

        public async Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
                .Where(a => a.EmployeeId == employeeId);

            if (startDate.HasValue)
                query = query.Where(a => a.CheckInTime >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(a => a.CheckInTime < endDate.Value.Date.AddDays(1));

            return await query.OrderBy(a => a.CheckInTime).ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync(DateTime? startDate, DateTime? endDate, int? employeeId)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(a => a.CheckInTime >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(a => a.CheckInTime < endDate.Value.Date.AddDays(1));

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            return await query.OrderBy(a => a.CheckInTime).ToListAsync();
        }

        public async Task<int> CountCheckedInTodayAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            return await _context.Attendances
                .Where(a => a.CheckInTime >= startOfDay && a.CheckInTime < endOfDay)
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();
        }
    }
}
