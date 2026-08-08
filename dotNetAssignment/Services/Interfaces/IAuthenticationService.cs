using System.Threading.Tasks;

using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Models.DTO.Login;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponseDto<AuthenticationResponseDto>> SignupAsync(SignupRequestDto SignupRequestDto);

        Task<ApiResponseDto<AuthenticationResponseDto>> LoginAsync(LoginRequestDto LoginRequestDto);

        Task<ApiResponseDto<string>> LogoutAsync(RefreshTokenDto refreshTokenDto);

        Task<ApiResponseDto<AuthenticationResponseDto>> TokenRefreshAsync(RefreshTokenDto refreshTokenDto);

    }
}
