using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Implementations
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(
            IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        /// <summary>
        /// Get all restaurants with pagination
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of restaurants to retrieve per page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<ApiResponseDto<RestaurantListResponseDto>> GetAllRestaurantsListAsync(int page, int pageSize)
        {
            var restaurants = await _restaurantRepository.GetAllRestaurantsAsync(page, pageSize);
            var totalCount = await _restaurantRepository.GetRestaurantCountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page > totalPages)
            {
                return new ApiResponseDto<RestaurantListResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.PageNotFound
                };
            }

            var response = new RestaurantListResponseDto
            {
                Restaurants = restaurants.Select(
                    x => new RestaurantResponseDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        AddressLineOne = x.AddressLineOne,
                        Landmark = x.Landmark,
                        City = x.City,
                        State = x.State,
                        Pincode = x.Pincode,
                        Rating = x.Rating,
                    }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return new ApiResponseDto<RestaurantListResponseDto>
            {
                Success = true,
                Data = response
            };
        }

        /// <summary>
        /// Get the menu list of a specific restaurant
        /// </summary>
        /// <param name="RestaurantId">The ID of the restaurant for which to retrieve the menu.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of menu items to retrieve per page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<ApiResponseDto<MenuListResponseDto>> GetMenuListAsync(Guid RestaurantId, int page, int pageSize)
        {
            var restaurantExists = await _restaurantRepository.RestaurantExistsAsync(RestaurantId);

            if (!restaurantExists)
            {
                return new ApiResponseDto<MenuListResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.RestaurantDoesntExists
                };
            }

            var menuItems = await _restaurantRepository.GetRestaurantMenuAsync(RestaurantId, page, pageSize);
            var totalCount = await _restaurantRepository.GetRestaurantMenuCountAsync(RestaurantId);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page > totalPages)
            {
                return new ApiResponseDto<MenuListResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.PageNotFound
                };
            }

            var response = new MenuListResponseDto
            {
                menu = menuItems.Select(
                    x => new MenuResponseDto
                    {
                        Id = x.Id,
                        DishName = x.DishName,
                        Price = x.Price,
                        Rating = x.Rating
                    }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return new ApiResponseDto<MenuListResponseDto>
            {
                Success = true,
                Data = response
            };
        }

    }
}