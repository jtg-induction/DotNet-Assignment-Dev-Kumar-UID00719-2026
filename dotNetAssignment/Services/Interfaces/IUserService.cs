using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponseDto<string>> UpdateUserAsync(Guid userId, UpdateUserRequestDto request);

        Task<ApiResponseDto<string>> AddAddressAsync(Guid userId, AddAddressRequestDto request);

        Task<ApiResponseDto<string>> UpdateAddressAsync(Guid userId, UpdateAddressRequestDto request);

        Task<ApiResponseDto<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);

        Task<ApiResponseDto<string>> DeactivateUserAsync(Guid userId);

    }
}
