using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Models.DTO.Login;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponseDto<AuthenticationResponseDto>> SignupAsync(SignupRequestDto SignupRequestDto);

        Task<ApiResponseDto<AuthenticationResponseDto>> LoginAsync(LoginRequestDto LoginRequestDto);

        Task<ApiResponseDto<string>> LogoutAsync(RefreshTokenRequestDto refreshTokenDto);

        Task<ApiResponseDto<AccessTokenRefreshResponse>> TokenRefreshAsync(RefreshTokenRequestDto refreshTokenDto);

    }
}
