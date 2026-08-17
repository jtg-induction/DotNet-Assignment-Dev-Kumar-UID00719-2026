using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Repositories.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestaurantDbContext _context;

        public OrderRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<bool> MenuItemExistsAsync(Guid MenuId)
        {
            return await _context.Menus.AnyAsync(x => x.Id == MenuId);
        }

        public async Task<Menu> GetMenuItemByIdAsync(Guid MenuId)
        {
            return await _context.Menus.SqlQuery(@"SELECT * FROM Menus With (UPDLOCK, ROWLOCK) WHERE Id = @p0", MenuId).FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid OrderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid OrderId)
        {
            return await _context.OrderItems.AsNoTracking().Include(oi=> oi.Menu).Where(x => x.OrderId == OrderId).ToListAsync();
        }

        public DbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}