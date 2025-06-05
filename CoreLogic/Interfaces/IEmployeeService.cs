using CoreLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeProfileDto> GetProfileAsync(int employeeId);
        Task UpdateSignatureAsync(int employeeId, SignatureDto signatureDto);
        Task<EmployeeListDto> GetEmployeeListAsync(EmployeeListQueryDto query);
        Task AddEmployeeAsync(AddEmployeeDto addEmployeeDto);
        Task EditEmployeeAsync(int employeeId, EditEmployeeDto editEmployeeDto);
        Task DeleteEmployeeAsync(int employeeId);
        Task<EditEmployeeDto> GetEmployeeByIdAsync(int employeeId);

    }
}
