using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.DTO.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponseDto<string>> UpdateUserAsync(Guid UserId, UpdateUserRequestDto request);

        Task<ApiResponseDto<string>> AddAddressAsync(Guid UserId, AddAddressRequestDto request);

        Task<ApiResponseDto<string>> UpdateAddressAsync(Guid UserId, UpdateAddressRequestDto request);

    }
}