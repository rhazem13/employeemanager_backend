using CoreLogic.DTOs;
using CoreLogic.Interfaces;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IAttendanceRepository attendanceRepository)
        {
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<EmployeeProfileDto> GetProfileAsync(int employeeId)
        {
            // Retrieve employee
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            // Retrieve attendance history
            var checkIns = await _attendanceRepository.GetByEmployeeIdAsync(employeeId);

            // Calculate weekly summaries
            var weeklySummaries = new List<WeeklySummary>();
            if (checkIns.Any())
            {
                var minDate = checkIns.Min(c => c.CheckInTime.Date);
                var maxDate = checkIns.Max(c => c.CheckInTime.Date);
                var currentWeekStart = minDate.AddDays(-(int)minDate.DayOfWeek + (int)DayOfWeek.Monday);

                while (currentWeekStart <= maxDate)
                {
                    var weekEnd = currentWeekStart.AddDays(7);
                    var weekCheckIns = checkIns
                        .Count(c => c.CheckInTime.Date >= currentWeekStart && c.CheckInTime.Date < weekEnd);
                    weeklySummaries.Add(new WeeklySummary
                    {
                        WeekStart = currentWeekStart,
                        CheckInCount = weekCheckIns
                    });
                    currentWeekStart = weekEnd;
                }
            }

            // Map to DTO
            return new EmployeeProfileDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                NationalId = employee.NationalId,
                Age = employee.Age,
                Signature = employee.Signature,
                Email = employee.Email,
                WeeklyAttendanceHistory = weeklySummaries
            };
        }

        public async Task UpdateSignatureAsync(int employeeId, SignatureDto signatureDto)
        {
            // Retrieve employee
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            // Check if signature is already set
            if (!string.IsNullOrEmpty(employee.Signature))
                throw new Exception("Signature already exists.");

            // Update signature
            employee.Signature = signatureDto.Signature;
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task<EmployeeListDto> GetEmployeeListAsync(EmployeeListQueryDto query)
        {
            // Retrieve employees with pagination, sorting, and filtering
            var employees = await _employeeRepository.GetAllAsync(query.PageNumber, query.PageSize, query.SortBy, query.SortDirection, query.Filter);

            // Get total count for pagination
            var totalCount = await _employeeRepository.CountAsync(query.Filter);

            // Map to DTO
            var employeeDtos = employees.Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                PhoneNumber = e.PhoneNumber,
                NationalId = e.NationalId,
                Age = e.Age,
                Email = e.Email,
                Role = e.Role
            }).ToList();

            return new EmployeeListDto
            {
                Employees = employeeDtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task AddEmployeeAsync(AddEmployeeDto addEmployeeDto)
        {
            // Check for existing email or national ID
            var existingEmail = await _employeeRepository.GetByEmailAsync(addEmployeeDto.Email);
            if (existingEmail != null)
                throw new Exception("Email is already in use.");

            var existingNationalId = await _employeeRepository.GetByNationalIdAsync(addEmployeeDto.NationalId);
            if (existingNationalId != null)
                throw new Exception("National ID is already in use.");

            // Create new employee
            var employee = new Employee
            {
                FirstName = addEmployeeDto.FirstName,
                LastName = addEmployeeDto.LastName,
                PhoneNumber = addEmployeeDto.PhoneNumber,
                NationalId = addEmployeeDto.NationalId,
                Age = addEmployeeDto.Age,
                Email = addEmployeeDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(addEmployeeDto.Password),
                Role = addEmployeeDto.Role,
                Signature = addEmployeeDto.Signature
            };

            await _employeeRepository.AddAsync(employee);
        }

        public async Task EditEmployeeAsync(int employeeId, EditEmployeeDto editEmployeeDto)
        {
            // Retrieve employee
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            // Check for email or national ID conflicts
            var existingEmail = await _employeeRepository.GetByEmailAsync(editEmployeeDto.Email);
            if (existingEmail != null && existingEmail.Id != employeeId)
                throw new Exception("Email is already in use by another employee.");

            var existingNationalId = await _employeeRepository.GetByNationalIdAsync(editEmployeeDto.NationalId);
            if (existingNationalId != null && existingNationalId.Id != employeeId)
                throw new Exception("National ID is already in use by another employee.");

            // Update employee
            employee.FirstName = editEmployeeDto.FirstName;
            employee.LastName = editEmployeeDto.LastName;
            employee.PhoneNumber = editEmployeeDto.PhoneNumber;
            employee.NationalId = editEmployeeDto.NationalId;
            employee.Age = editEmployeeDto.Age;
            employee.Email = editEmployeeDto.Email;
            employee.Role = editEmployeeDto.Role;
            employee.Signature = editEmployeeDto.Signature;

            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            // Retrieve employee
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            await _employeeRepository.DeleteAsync(employeeId);
        }

        public async Task<EditEmployeeDto> GetEmployeeByIdAsync(int employeeId)
        {
            // Retrieve employee
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found.");

            // Map to EditEmployeeDto
            return new EditEmployeeDto
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                NationalId = employee.NationalId,
                Age = employee.Age,
                Email = employee.Email,
                Role = employee.Role,
                Signature = employee.Signature
            };
        }
    }
}
