using BCrypt.Net;
using CoreLogic.DTOs;
using CoreLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
//using DataAccess.Repositories;
using Models.Entities;
using System;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static CoreLogic.DTOs.AuthDtos;
namespace CoreLogic.Services
{
    public class AuthService: IAuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }

        public async Task<TokenResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email or national ID already exists
            var existingEmployeeByEmail = await _employeeRepository.GetByEmailAsync(registerDto.Email);
            if (existingEmployeeByEmail != null)
                throw new Exception("Email already exists.");

            var existingEmployeeByNationalId = await _employeeRepository.GetByNationalIdAsync(registerDto.NationalId);
            if (existingEmployeeByNationalId != null)
                throw new Exception("National ID already exists.");

            // Create new employee
            var employee = new Employee
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                NationalId = registerDto.NationalId,
                Age = registerDto.Age,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = registerDto.Role
            };

            await _employeeRepository.AddAsync(employee);

            // Generate JWT token
            var token = GenerateJwtToken(employee);
            return new TokenResponseDto
            {
                Token = token,
                Role = employee.Role.ToString(),
                EmployeeId = employee.Id
            };
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            var employee = await _employeeRepository.GetByEmailAsync(loginDto.Email);
            if (employee == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, employee.PasswordHash))
                throw new Exception("Invalid email or password.");

            var token = GenerateJwtToken(employee);
            return new TokenResponseDto
            {
                Token = token,
                Role = employee.Role.ToString(),
                EmployeeId = employee.Id
            };
        }

        private string GenerateJwtToken(Employee employee)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Role, employee.Role.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
