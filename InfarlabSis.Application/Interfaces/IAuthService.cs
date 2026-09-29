using InfarlabSis.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfarlabSis.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
