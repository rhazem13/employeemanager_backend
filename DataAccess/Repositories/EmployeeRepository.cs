using CoreLogic.Interfaces;
using DataAccess.Data;
using Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Employee?> GetByNationalIdAsync(string nationalId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.NationalId == nationalId);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(int pageNumber, int pageSize, string? sortBy, string? filter)
        {
            var query = _context.Employees.AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(e => e.FirstName.Contains(filter) ||
                                        e.LastName.Contains(filter) ||
                                        e.NationalId.Contains(filter) ||
                                        e.PhoneNumber.Contains(filter));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "firstname":
                        query = query.OrderBy(e => e.FirstName);
                        break;
                    case "lastname":
                        query = query.OrderBy(e => e.LastName);
                        break;
                    case "age":
                        query = query.OrderBy(e => e.Age);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id);
                        break;
                }
            }

            // Apply pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
