using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Repositories.RestaurantRepo
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetAllRestaurantsAsync();

        Task<bool> RestaurantExistsAsync(Guid RestaurantId);

        Task<Restaurant> GetRestaurantByIdAsync(Guid RestaurantId);

        Task<List<Menu>> GetRestaurantMenuAsync(Guid RestaurantId);
    }
}