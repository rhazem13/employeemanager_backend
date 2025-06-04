using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using CoreLogic.DTOs;
using CoreLogic.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Employee")] // Restrict to Employee role
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IValidator<CheckInDto> _checkInValidator;

        public AttendanceController(IAttendanceService attendanceService, IValidator<CheckInDto> checkInValidator)
        {
            _attendanceService = attendanceService;
            _checkInValidator = checkInValidator;
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto checkInDto)
        {
            // Validate DTO
            var validationResult = await _checkInValidator.ValidateAsync(checkInDto);
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

                await _attendanceService.CheckInAsync(employeeId, checkInDto);
                return Ok(new { message = "Check-in successful." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetCheckInHistory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                // Extract employee ID from JWT token
                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out var employeeId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var history = await _attendanceService.GetCheckInHistoryAsync(employeeId, startDate, endDate);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
