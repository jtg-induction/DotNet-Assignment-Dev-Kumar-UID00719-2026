using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        Task<bool> MenuItemExistsAsync(Guid MenuId);

        Task<Menu> GetMenuItemByIdAsync(Guid MenuId);

        void AddOrder(Order order);

        void AddOrderItem(OrderItem orderItem);

        Task SaveChangesAsync();
    }
}