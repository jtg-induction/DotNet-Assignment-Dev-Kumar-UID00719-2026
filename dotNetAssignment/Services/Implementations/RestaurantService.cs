using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
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
        private readonly IUserRepository _userRepository;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IUserRepository userRepository)
        {
            _restaurantRepository = restaurantRepository;
            _userRepository = userRepository;
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

        public async Task<ApiResponseDto<CreateRestaurantResponseDto>> CreateRestaurantAsync(CreateRestaurantRequestDto request)
        {
            var user = await _userRepository.GetUserByIdAsync(request.OwnerId);

            if (user == null || !user.IsActive)
            {
                return new ApiResponseDto<CreateRestaurantResponseDto>
                {
                    Success = false,
                    Message = ExceptionMessages.UserNotFound
                };
            }

            if(user.Role == UserRole.Customer)
            {
                user.Role = UserRole.Owner;
            }

            var restaurant = new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                AddressLineOne = request.AddressLineOne,
                Landmark = request.Landmark,
                City = request.City,
                State = request.State,
                Pincode = request.Pincode,
                Rating = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var restaurantOwner = new RestaurantOwner
            {
                UserId = user.Id,
                RestaurantId = restaurant.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _restaurantRepository.AddRestaurantAsync(restaurant);
            await _restaurantRepository.AddRestaurantOwnerAsync(restaurantOwner);

            await _restaurantRepository.SaveChangesAsync();

            // Implementation for creating restaurant
            return new ApiResponseDto<CreateRestaurantResponseDto>
            {
                Success = true,
                Data = new CreateRestaurantResponseDto()
                {
                    RestaurantId = restaurant.Id
                }
            };
        }

    }
}