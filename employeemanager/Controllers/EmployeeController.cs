using CoreLogic.DTOs;
using CoreLogic.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IValidator<SignatureDto> _signatureValidator;
        private readonly IValidator<EmployeeListQueryDto> _employeeListQueryValidator;
        private readonly IValidator<AddEmployeeDto> _addEmployeeValidator;
        private readonly IValidator<EditEmployeeDto> _editEmployeeValidator;

        public EmployeeController(
            IEmployeeService employeeService,
            IValidator<SignatureDto> signatureValidator,
            IValidator<EmployeeListQueryDto> employeeListQueryValidator,
            IValidator<AddEmployeeDto> addEmployeeValidator,
            IValidator<EditEmployeeDto> editEmployeeValidator)
        {
            _employeeService = employeeService;
            _signatureValidator = signatureValidator;
            _employeeListQueryValidator = employeeListQueryValidator;
            _addEmployeeValidator = addEmployeeValidator;
            _editEmployeeValidator = editEmployeeValidator;
        }

        [HttpGet("profile")]
        [Authorize(Policy = "Employee")] // Restrict to Employee role
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Extract employee ID from JWT token
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out var employeeId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var profile = await _employeeService.GetProfileAsync(employeeId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("personal-data")]
        [Authorize(Policy = "Employee")]
        public async Task<IActionResult> GetPersonalData()
        {
            try
            {
                // Extract employee ID from JWT token
                var employeeIdClaim = await VerifyEmployeeIdAsync();
                if (!employeeIdClaim.HasValue)
                {
                    return Unauthorized(new { message = employeeIdClaim.ErrorMessage });
                }

                var personalData = await _employeeService.GetPersonalDataAsync(employeeIdClaim.Value);
                return Ok(personalData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("signature")]
        [Authorize(Policy = "Employee")] // Restrict to Employee role
        public async Task<IActionResult> UpdateSignature([FromBody] SignatureDto signatureDto)
        {
            // Validate DTO
            var validationResult = await _signatureValidator.ValidateAsync(signatureDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            try
            {
                // Extract employee ID from JWT token
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out var employeeId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                await _employeeService.UpdateSignatureAsync(employeeId, signatureDto);
                return Ok(new { message = "Signature updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("list")]
        [Authorize(Policy = "Admin")] // Restrict to Admin role
        public async Task<IActionResult> GetEmployeeList([FromQuery] EmployeeListQueryDto query)
        {
            // Validate query parameters
            var validationResult = await _employeeListQueryValidator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            try
            {
                var employeeList = await _employeeService.GetEmployeeListAsync(query);
                return Ok(employeeList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDto addEmployeeDto)
        {
            // Validate DTO
            var validationResult = await _addEmployeeValidator.ValidateAsync(addEmployeeDto);
            if (!validationResult.IsValid)
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            try
            {
                await _employeeService.AddEmployeeAsync(addEmployeeDto);
                return Ok(new { message = "Employee added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> EditEmployee(int id, [FromBody] EditEmployeeDto editEmployeeDto)
        {
            // Validate DTO
            var validationResult = await _editEmployeeValidator.ValidateAsync(editEmployeeDto);
            if (!validationResult.IsValid)
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            try
            {
                await _employeeService.EditEmployeeAsync(id, editEmployeeDto);
                return Ok(new { message = "Employee updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                return Ok(new { message = "Employee deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<(bool HasValue, int Value, string? ErrorMessage)> VerifyEmployeeIdAsync()
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out var employeeId))
                return (false, 0, "Invalid token.");
            return (true, employeeId, null);
        }
    }
}
