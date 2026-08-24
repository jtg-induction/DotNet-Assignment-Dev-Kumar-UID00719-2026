using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Login;
using dotNetAssignment.Models.DTO.SignUp;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Repositories.Jwt;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
                    Message = ExceptionMessages.EmailAlreadyExists
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Role = UserRole.Customer,
                Email = request.Email.ToLowerInvariant(),
                Password = _passwordService.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Balance = Constants.Constants.DefaultUserBalance,
                PhoneNumber = request.PhoneNumber,
                UpdatedAt = DateTime.UtcNow,
            };

            _userRepository.AddUser(user);

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
            _jwtRepository.AddJwtId(refreshToken.JwtId);

            await _jwtRepository.SaveChangesAsync();
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = SuccessMessages.UserCreated,
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
                    Message = ExceptionMessages.InvalidEmailOrPassword
                };
            }

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
            _jwtRepository.AddJwtId(refreshToken.JwtId);
            await _jwtRepository.SaveChangesAsync();

            return new ApiResponseDto<AuthenticationResponseDto>
            {
                Success = true,
                Message = SuccessMessages.UserLoggedIn,
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
            ClaimsPrincipal principal;

            try
            {
                principal = _jwtService.ValidateRefreshToken(request.RefreshToken);
            }
            catch (SecurityTokenException)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRefreshToken
                };
            }

            var jwtId = _jwtService.GetJwtId(principal);

            if (!await _jwtRepository.JwtIdExistsAsync(jwtId))
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRefreshToken
                };
            }

            await _jwtRepository.RemoveJwtIdAsync(jwtId);
            await _jwtRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.UserLoggedOut
            };
        }

        /// <summary>
        /// Handles token refresh by validating the provided refresh token, generating a new access token, and returning it in the response.
        /// </summary>
        /// <param name="request">The refresh token request DTO.</param>
        /// <returns>
        /// An API response containing the refreshed authentication tokens on success
        /// or an error message when token refresh fails
        /// </returns>
        public async Task<ApiResponseDto<AccessTokenRefreshResponse>> TokenRefreshAsync(RefreshTokenRequestDto request)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _jwtService.ValidateRefreshToken(request.RefreshToken);
            }
            catch (SecurityTokenExpiredException)
            {
                var ExpiredjwtId = _jwtService.GetJwtIdFromExpiredToken(request.RefreshToken);
                if(ExpiredjwtId != null)
                {
                    await _jwtRepository.RemoveJwtIdAsync(ExpiredjwtId.Value);
                    await _jwtRepository.SaveChangesAsync();
                }
                return new ApiResponseDto<AccessTokenRefreshResponse>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRefreshToken
                };
            }
            catch (SecurityTokenException)
            {
                return new ApiResponseDto<AccessTokenRefreshResponse>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRefreshToken
                };
            }

            var jwtId = _jwtService.GetJwtId(principal);

            if (!await _jwtRepository.JwtIdExistsAsync(jwtId))
            {
                return new ApiResponseDto<AccessTokenRefreshResponse>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRefreshToken
                };
            }

            var userId = _jwtService.GetUserId(principal);
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                await _jwtRepository.RemoveJwtIdAsync(jwtId);
                await _jwtRepository.SaveChangesAsync();

                return new ApiResponseDto<AccessTokenRefreshResponse>
                {
                    Success = false,
                    Message = ExceptionMessages.UserNotFound
                };
            }

            var accessToken = _jwtService.GenerateAccessToken(user.Id,user.Email,user.Role);
                
            return new ApiResponseDto<AccessTokenRefreshResponse>
            {
                Success = true,
                Message = SuccessMessages.TokenRefreshed,
                Data = new AccessTokenRefreshResponse
                {
                    AccessToken = accessToken
                }
            };
            
        }
    }
}
