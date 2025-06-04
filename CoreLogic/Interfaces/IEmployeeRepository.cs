using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByEmailAsync(string email);
        Task<Employee?> GetByNationalIdAsync(string nationalId);
        Task AddAsync(Employee employee);
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync(int pageNumber, int pageSize, string? sortBy, string? filter);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
    }
}
