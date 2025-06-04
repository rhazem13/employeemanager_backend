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
    }
}
