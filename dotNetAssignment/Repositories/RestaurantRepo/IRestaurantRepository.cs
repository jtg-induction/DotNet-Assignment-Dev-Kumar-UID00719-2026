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

        Task<bool> RestaurantExistsAsync(Guid RestaurantId);

        Task<Restaurant> GetRestaurantByIdAsync(Guid RestaurantId);

        Task<List<Restaurant>> GetAllRestaurantsAsync(int page, int pageSize);

        Task<int> GetRestaurantCountAsync();

        Task<List<Menu>> GetRestaurantMenuAsync(Guid restaurantId, int page, int pageSize);

        Task<int> GetRestaurantMenuCountAsync( Guid restaurantId);

        Task AddRestaurantOwnerAsync(RestaurantOwner restaurantOwner);

        Task AddRestaurantAsync(Restaurant restaurant);

        Task SaveChangesAsync();

    }
}