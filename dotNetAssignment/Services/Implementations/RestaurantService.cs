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
        public async Task<ApiResponseDto<RestaurantListResponseDto>> GetAllRestaurantsListAsync()
        {
            var restaurants = await _restaurantRepository.GetAllRestaurantsAsync();

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
                    }).ToList()
            };

            return new ApiResponseDto<RestaurantListResponseDto>
            {
                Success = true,
                Data = response
            };
        }

        public async Task<ApiResponseDto<MenuListResponseDto>> GetMenuListAsync(Guid RestaurantId)
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

            var menuItems = await _restaurantRepository.GetRestaurantMenuAsync(RestaurantId);

            var response = new MenuListResponseDto
            {
                menu = menuItems.Select(
                    x => new MenuResponseDto
                    {
                        Id = x.Id,
                        DishName = x.DishName,
                        Price = x.Price,
                        Rating = x.Rating
                    }).ToList()
            };

            return new ApiResponseDto<MenuListResponseDto>
            {
                Success = true,
                Data = response
            };
        }

    }
}