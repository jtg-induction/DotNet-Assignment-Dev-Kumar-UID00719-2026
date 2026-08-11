using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Implementations
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository; 
        public readonly IPasswordService _passwordService;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService
            )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<ApiResponseDto<string>> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if(user == null || !user.IsActive)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                user.Name = request.Name;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = "User updated successfully"
            };
        }

        public async Task<ApiResponseDto<string>> AddAddressAsync(Guid userId, AddAddressRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

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
                Message = "Address added successfully."
            };
        }

        public async Task<ApiResponseDto<string>> UpdateAddressAsync(Guid userId, UpdateAddressRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var address = await _userRepository.GetAddressByIdAsync(request.AddressId);

            if (address == null)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "Address not found."
                };
            }

            if (!string.IsNullOrWhiteSpace(request.LineOne))
            {
                address.LineOne = request.LineOne;
            }

            if (!string.IsNullOrWhiteSpace(request.Landmark))
            {
                address.Landmark = request.Landmark;
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                address.City = request.City;
            }

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                address.State = request.State;
            }

            if (!string.IsNullOrWhiteSpace(request.Pincode))
            {
                address.Pincode = request.Pincode;
            }

            address.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = "Address updated successfully."
            };
        }

        public async Task<ApiResponseDto<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if(!_passwordService.VerifyPassword(user.Password, request.OldPassword))
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "Wrong password."
                };
            }

            user.Password = request.NewPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = "Password changed successfully."
            };
        }

        public async Task<ApiResponseDto<string>> DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                Success = true,
                Message = "User deactivated successfully."
            };
        }

    }
}
