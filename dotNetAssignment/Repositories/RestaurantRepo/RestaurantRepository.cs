using dotNetAssignment.Data;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Repositories.RestaurantRepo
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all restaurants with pagination
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of restaurants to retrieve per page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<List<Restaurant>> GetAllRestaurantsAsync(int page, int pageSize)
        {
            return await _context.Restaurants
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Get the total count of restaurants in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<int> GetRestaurantCountAsync()
        {
            return await _context.Restaurants.CountAsync();
        }

        /// <summary>
        /// Checks if a restaurant with the specified ID exists in the database.
        /// </summary>
        /// <param name="RestaurantId">The ID of the restaurant to check.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<bool> RestaurantExistsAsync(Guid RestaurantId)
        {
            return await _context.Restaurants.AnyAsync(x => x.Id == RestaurantId);
        }

        /// <summary>
        /// Gets the menu items for a specific restaurant with pagination.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant for which to retrieve the menu.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of menu items to retrieve per page.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<List<Menu>> GetRestaurantMenuAsync(Guid restaurantId, int page, int pageSize)
        {
            return await _context.Menus
                .Where(x => x.RestaurantId == restaurantId)
                .OrderBy(x => x.DishName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the total count of menu items for a specific restaurant.
        /// </summary>
        /// <param name="restaurantId">The ID of the restaurant for which to retrieve the menu count.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<int> GetRestaurantMenuCountAsync(Guid restaurantId)
        {
            return await _context.Menus
                .CountAsync(x => x.RestaurantId == restaurantId);
        }

        /// <summary>
        /// Gets a restaurant by its ID.
        /// </summary>
        /// <param name="RestaurantId">The ID of the restaurant to retrieve.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<Restaurant> GetRestaurantByIdAsync(Guid RestaurantId) 
        {
            return await _context.Restaurants.FirstOrDefaultAsync(x => x.Id == RestaurantId);
        }

        /// <summary>
        /// Saves all changes made in the context to the database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
