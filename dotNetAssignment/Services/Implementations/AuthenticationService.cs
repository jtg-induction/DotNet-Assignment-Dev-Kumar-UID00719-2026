using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Login;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.Jwt;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;

namespace dotNetAssignment.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IJwtRepository _jwtRepository;

        public AuthenticationService(
            IPasswordService passwordService,
            IUserRepository userRepository,
            IJwtService jwtService,
            IJwtRepository jwtRepository)
        {
            _passwordService = passwordService;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _jwtRepository = jwtRepository;

        }

        /// <summary>
        /// Handles user signup by creating a new user, hashing the password, generating JWT tokens, and saving the user to the repository.
        /// </summary>
        /// <param name="request">The signup request DTO.</param>
        /// <returns>
        /// An API response containing the authentication tokens on success
        /// or an error message when signup fails
        /// </returns>
        public async Task<ApiResponseDto<AuthenticationResponseDto>> SignupAsync(SignupRequestDto request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return new ApiResponseDto<AuthenticationResponseDto>
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Role = request.Role,
                Email = request.Email,
                Password = _passwordService.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Balance = 1000m,
                PhoneNumber = request.PhoneNumber,
                UpdatedAt = DateTime.UtcNow,
            };

            _userRepository.AddUser(user);

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
            _jwtRepository.AddJwtId(refreshToken.JwtId);

            try
            {
            await _jwtRepository.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                throw;
            }

            return new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = "User registered successfully",
                Data = new AuthenticationResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RefreshToken
                }
            };
        }

        /// <summary>
        /// Handles user login by verifying credentials, generating JWT tokens, and returning them in the response.
        /// </summary>
        /// <param name="request">The login request DTO.</param>
        /// <returns>
        /// An API response containing the authentication tokens on success
        /// or an error message when login fails
        /// </returns>
        public async Task<ApiResponseDto<AuthenticationResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !_passwordService.VerifyPassword(request.Password, user.Password))
            {
                return new ApiResponseDto<AuthenticationResponseDto>
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
            _jwtRepository.AddJwtId(refreshToken.JwtId);
            await _jwtRepository.SaveChangesAsync();

            return new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthenticationResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RefreshToken
                }
            };
        }

        /// <summary>
        /// Handles user logout by validating the refresh token, removing the associated JWT ID from the repository, and returning a success or error response.
        /// </summary>
        /// <param name="request">The refresh token request DTO.</param>
        /// <returns>
        /// An API response indicating the success or failure of the logout operation
        /// </returns>
        public async Task<ApiResponseDto<string>> LogoutAsync(RefreshTokenRequestDto request)
        {
            try
            {
                var principal = _jwtService.ValidateRefreshToken(request.RefreshToken);
                var jwtId = _jwtService.GetJwtId(principal);

                if (!await _jwtRepository.JwtIdExistsAsync(jwtId))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid refresh token."
                    };
                }

                await _jwtRepository.RemoveJwtIdAsync(jwtId);
                await _jwtRepository.SaveChangesAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Logged out successfully."
                };
            }
            catch
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }
        }

        /// <summary>
        /// Handles token refresh by validating the provided refresh token, generating a new access token, and returning it in the response.
        /// </summary>
        /// <param name="request">The refresh token request DTO.</param>
        /// <returns>
        /// An API response containing the refreshed authentication tokens on success
        /// or an error message when token refresh fails
        /// </returns>
        public async Task<ApiResponseDto<AuthenticationResponseDto>> TokenRefreshAsync(RefreshTokenRequestDto request)
        {
            try
            {
                var principal = _jwtService.ValidateRefreshToken(request.RefreshToken);
                var jwtId = _jwtService.GetJwtId(principal);

                if (!await _jwtRepository.JwtIdExistsAsync(jwtId))
                {
                    return new ApiResponseDto<AuthenticationResponseDto>
                    {
                        Success = false,
                        Message = "Invalid refresh token."
                    };
                }

                var userId = _jwtService.GetUserId(principal);
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null || !user.IsActive)
                {
                    await _jwtRepository.RemoveJwtIdAsync(jwtId);
                    await _jwtRepository.SaveChangesAsync();

                    return new ApiResponseDto<AuthenticationResponseDto>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                var accessToken = _jwtService.GenerateAccessToken(user.Id,user.Email,user.Role);
                
                return new ApiResponseDto<AuthenticationResponseDto>
                {
                    Success = true,
                    Message = "Token refreshed successfully.",
                    Data = new AuthenticationResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = request.RefreshToken
                    }
                };
            }
            catch
            {
                return new ApiResponseDto<AuthenticationResponseDto>
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }
        }
    }
}
