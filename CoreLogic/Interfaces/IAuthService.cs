using CoreLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CoreLogic.Interfaces
{
    public  interface IAuthService
    {
        Task<AuthDtos.TokenResponseDto> RegisterAsync(AuthDtos.RegisterDto registerDto);
        Task<AuthDtos.TokenResponseDto> LoginAsync(AuthDtos.LoginDto loginDto);
    }
}
