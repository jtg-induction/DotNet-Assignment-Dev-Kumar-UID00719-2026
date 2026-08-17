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
        Task<bool> MenuItemExistsAsync(Guid MenuId);

        Task<Menu> GetMenuItemByIdAsync(Guid MenuId);

        Task<Order> GetOrderByIdAsync(Guid OrderId);

        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid OrderId);

        DbContextTransaction BeginTransaction();

        void AddOrder(Order order);

        void AddOrderItem(OrderItem orderItem);

        Task SaveChangesAsync();
    }
}