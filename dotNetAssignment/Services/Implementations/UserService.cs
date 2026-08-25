using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.Jwt;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Implementations
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository; 
        public readonly IPasswordService _passwordService;
        public readonly IJwtService _jwtService;
        public readonly IJwtRepository _jwtRepository;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IJwtRepository jwtRepository
            )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _jwtRepository = jwtRepository;
        }

        /// <summary>
        /// Updates the user information based on the provided request.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="request">The request containing the updated user information.</param>
        /// <returns>An ApiResponseDto indicating the success or failure of the operation.</returns>
        public async Task<ApiResponseDto<string>> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var updated = false;

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                user.Name = request.Name;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (user.PhoneNumber == request.PhoneNumber)
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = ExceptionMessages.SamePhoneNumber
                    };
                }

                user.PhoneNumber = request.PhoneNumber;
                updated = true;
            }

            if (updated)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = SuccessMessages.UserUpdated
                };
            }
            else
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.UserNotUpdated
                };
            }
        }

        /// <summary>
        /// Adds a new address for the specified user based on the provided request.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to add an address.</param>
        /// <param name="request">The request containing the address.</param>
        /// <returns>An ApiResponseDto indicating the success or failure of the operation.</returns>
        public async Task<ApiResponseDto<string>> AddAddressAsync(Guid userId, AddAddressRequestDto request)
        {
            var address = new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LineOne = request.LineOne,
                Landmark = request.Landmark,
                Pincode = request.Pincode,
                City = request.City,
                State = request.State,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _userRepository.AddAddress(address);
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.AddressAdded
            };
        }

        /// <summary>
        /// Updates the address for the specified user based on the provided request.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to update an address.</param>
        /// <param name="request">The request containing the updated address.</param>
        /// <returns>An ApiResponseDto indicating the success or failure of the operation.</returns>
        public async Task<ApiResponseDto<string>> UpdateAddressAsync(Guid userId, UpdateAddressRequestDto request)
        {
            var updated = false;

            var address = await _userRepository.GetAddressByIdAsync(request.AddressId);

            if (address == null || address.UserId != userId)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.AddressNotFound
                };
            }

            if (!string.IsNullOrWhiteSpace(request.LineOne))
            {
                address.LineOne = request.LineOne;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Landmark))
            {
                address.Landmark = request.Landmark;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                address.City = request.City;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                address.State = request.State;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Pincode))
            {
                address.Pincode = request.Pincode;
                updated = true;
            }

            if (updated)
            {
                address.UpdatedAt = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();
            }

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.AddressUpdated
            };
        }

        /// <summary>
        /// Changes the password for the specified user based on the provided request.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to change the password.</param>
        /// <param name="request">The request containing the old and new password information.</param>
        /// <returns>An ApiResponseDto indicating the success or failure of the operation.</returns>
        public async Task<ApiResponseDto<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if(!_passwordService.VerifyPassword(request.OldPassword, user.Password))
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.WrongPassword
                };
            }

            if (_passwordService.VerifyPassword(request.NewPassword, user.Password))
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.SamePassword
                };
            }

            user.Password = _passwordService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.PasswordChanged
            };
        }

        /// <summary>
        /// Deactivates the specified user based on the provided user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate.</param>
        /// <param name="request">The request containing the refresh token.</param>
        /// <returns>An ApiResponseDto indicating the success or failure of the operation.</returns>
        public async Task<ApiResponseDto<string>> DeactivateUserAsync(Guid userId, DeactivateAccountRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
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

            var tokenUserId = _jwtService.GetUserId(principal);

            if (tokenUserId != userId)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRefreshToken
                };
            }

            var jwtId = _jwtService.GetJwtId(principal);
            await _jwtRepository.RemoveJwtIdAsync(jwtId);

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepository.SaveChangesAsync();
            await _jwtRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = SuccessMessages.UserDeactivated
            };
        }

    }
}
