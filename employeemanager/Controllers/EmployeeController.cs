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
    [Authorize(Policy = "Employee")] // Restrict to Employee role
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IValidator<SignatureDto> _signatureValidator;

        public EmployeeController(IEmployeeService employeeService, IValidator<SignatureDto> signatureValidator)
        {
            _employeeService = employeeService;
            _signatureValidator = signatureValidator;
        }

        [HttpGet("profile")]
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

        [HttpPost("signature")]
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
    }
}
