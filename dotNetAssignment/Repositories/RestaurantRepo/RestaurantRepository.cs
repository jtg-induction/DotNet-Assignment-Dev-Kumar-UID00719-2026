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

        public async Task<List<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _context.Restaurants.ToListAsync();
        }

        public async Task<bool> RestaurantExistsAsync(Guid RestaurantId)
        {
            return await _context.Restaurants.AnyAsync(x => x.Id == RestaurantId);
        }

        public async Task<List<Menu>> GetRestaurantMenuAsync(Guid RestaurantId)
        {
            return await _context.Menus.Where(x => x.RestaurantId == RestaurantId).ToListAsync();
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(Guid RestaurantId) 
        {
            return await _context.Restaurants.FirstOrDefaultAsync(x => x.Id == RestaurantId);
        }
    }
}