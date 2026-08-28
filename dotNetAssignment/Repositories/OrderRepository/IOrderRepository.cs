using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        Task<bool> MenuItemExistsAsync(Guid menuId);

        Task<Menu> GetMenuItemByIdAsync(Guid menuId);

        Task<List<Menu>> GetAllMenuItemsByOrderIdAsync(List<Guid> menuItemIds);

        Task<Order> GetOrderByIdAsync(Guid orderId);

        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);

        Task<List<Order>> GetOrdersForDashboardAsync(DashboardOrderListRequestDto request, List<Guid> restaurantIds);

        Task<int> GetDashboardOrdersCountAsync(DashboardOrderListRequestDto request, List<Guid> restaurantIds);

        DbContextTransaction BeginTransaction();

        void AddOrder(Order order);

        void AddOrderItem(OrderItem orderItem);

        Task SaveChangesAsync();
    }
}
