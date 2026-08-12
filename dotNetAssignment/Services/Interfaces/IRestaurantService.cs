using dotNetAssignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<ApiResponseDto<RestaurantListResponseDto>> GetAllRestaurantsListAsync();
        Task<ApiResponseDto<MenuListResponseDto>> GetMenuListAsync(Guid RestaurantId);

    }
}