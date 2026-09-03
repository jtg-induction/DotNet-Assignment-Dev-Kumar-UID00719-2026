using dotNetAssignment.Data;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
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

        /// <summary>
        /// Checks if a menu item with the specified ID exists in the database.
        /// </summary>
        /// <param name="menuId">menuId of the menu item to check</param>
        /// <returns>Boolean indicating if the menu item exists</returns>
        public async Task<bool> MenuItemExistsAsync(Guid menuId)
        {
            return await _context.Menus.AnyAsync(x => x.Id == menuId);
        }

        /// <summary>
        /// Retrieves a menu item by its ID with an exclusive lock to prevent concurrent modifications.
        /// </summary>
        /// <param name="menuId">menuId of the menu item to retrieve</param>
        /// <returns>Menu item if found, otherwise null</returns>
        public async Task<Menu> GetMenuItemByIdAsync(Guid menuId)
        {
            return await _context.Menus.SqlQuery(@"SELECT * FROM Menus With (UPDLOCK, ROWLOCK) WHERE Id = @p0", menuId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all menu items by a list of IDs with an exclusive lock to prevent concurrent modifications.
        /// </summary>
        /// <param name="menuItemIds">List of menu item IDs to retrieve</param>
        /// <returns>List of menu items if found, otherwise empty list</returns>
        public async Task<List<Menu>> GetAllMenuItemsByOrderIdAsync(List<Guid> menuItemIds)
        {
            if(menuItemIds == null || !menuItemIds.Any())
            {
                return new List<Menu>(); 
            }

            var idString = string.Join(",", menuItemIds.Select(id => $"'{id}'"));
            return await _context.Menus.SqlQuery($"SELECT * FROM Menus With (UPDLOCK, ROWLOCK) WHERE Id IN ({idString})").ToListAsync();
        }

        /// <summary>
        /// Retrieves an order by its ID, including the associated restaurant information.
        /// </summary>
        /// <param name="orderId">ID of the order to retrieve</param>
        /// <returns>Order if found, otherwise null</returns>
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders.Include(oi => oi.Restaurant).FirstOrDefaultAsync(oi => oi.Id == orderId);
        }

        /// <summary>
        /// Retrieves all order items associated with a specific order ID, including the associated menu information.
        /// </summary>
        /// <param name="orderId">ID of the order for which to retrieve items</param>
        /// <returns>List of order items if found, otherwise empty list</returns>
        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderItems.AsNoTracking().Include(oi=> oi.Menu).Where(x => x.OrderId == orderId).ToListAsync();
        }

        /// <summary>
        /// Get list of orders matching the specified dashboard filters
        /// </summary>
        /// <param name="request">Contains filtering, pagination, searching and sorting parameters</param>
        /// <param name="restaurantIds">List of Id of the restaurants belonging to the owner</param>
        /// <returns>List or orders</returns>
        public async Task<List<Order>> GetOrdersForDashboardAsync(DashboardOrderListRequestDto request, List<Guid> restaurantIds)
        {
            var query = _context.Orders.AsNoTracking().Include(o => o.User).Include(o => o.Restaurant).Where(o => restaurantIds.Contains(o.RestaurantId));
        
            if(request.Status != null)
            {
                query = query.Where(o => o.Status == request.Status);
            }
            if (request.FromDate != null)
            {
                var fromDate = request.FromDate.Value.Date;
                query = query.Where(o => o.PlacedAt >= fromDate);
            }
            if (request.ToDate != null)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(o => o.PlacedAt < toDate);
            }

            //else
            //{
            //    var currentDate = DateTime.UtcNow.Date;
            //    var tommorow = currentDate.AddDays(1);
            //    query = query.Where(o => o.PlacedAt >= currentDate && o.PlacedAt < tommorow);
            //}

            if (!string.IsNullOrWhiteSpace(request.SearchOrderIds))
            {
                var searchOrderIds = request.SearchOrderIds.Split(',').Select(id => Guid.Parse(id.Trim())).ToList();
                query = query.Where(o => searchOrderIds.Contains(o.Id));
            }

            if (request.SortBy == SortOrdersFields.Status)
            {
                var statusOrder = query.Select(o => new
                {
                    Order = o,
                    StatusPriority =
                        o.Status == OrderStatus.Dispatched ? 1 :
                        o.Status == OrderStatus.Accepted ? 2 :
                        o.Status == OrderStatus.Placed ? 3 :
                        o.Status == OrderStatus.Delivered ? 4 :
                        o.Status == OrderStatus.Rejected ? 5 :
                        6
                });

                if (request.SortOrder == "asc")
                {
                    query = statusOrder
                        .OrderBy(x => x.StatusPriority)
                        .Select(x => x.Order);
                }
                else
                {
                    query = statusOrder
                        .OrderByDescending(x => x.StatusPriority)
                        .Select(x => x.Order);
                }
            }
            else
            {
                if (request.SortOrder == "asc") 
                {
                    query = query.OrderBy(o => o.PlacedAt);
                }
                else
                {
                    query = query.OrderByDescending(o => o.PlacedAt);
                }
            }

            return await query.Skip((request.Page-1) * request.PageSize).Take(request.PageSize).ToListAsync();
        }

        /// <summary>
        /// Gets the total number of orders matching the specified dashboard filters
        /// </summary>
        /// <param name="request">Containing the filter parameters</param>
        /// <param name="restaurantIds">List of Id of the restaurants belonging to the owner</param>
        /// <returns>Count of orders</returns>
        public async Task<int> GetDashboardOrdersCountAsync(DashboardOrderListRequestDto request, List<Guid> restaurantIds)
        {
            var query = _context.Orders.AsNoTracking().Where(o => restaurantIds.Contains(o.RestaurantId));

           if (request.Status != null)
            {
                query = query.Where(o => o.Status == request.Status);
            }
            if (request.FromDate != null)
            {
                var fromDate = request.FromDate.Value.Date;
                query = query.Where(o => o.PlacedAt >= fromDate);
            }
            if (request.ToDate != null)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(o => o.PlacedAt < toDate);
            }
            //else
            //{
            //    var currentDate = DateTime.UtcNow.Date;
            //    var tommorow = currentDate.AddDays(1);
            //    query = query.Where(o => o.PlacedAt >= currentDate && o.PlacedAt < tommorow);
            //}

            if (!string.IsNullOrWhiteSpace(request.SearchOrderIds))
            {
                var searchOrderIds = request.SearchOrderIds.Split(',').Select(id => Guid.Parse(id.Trim())).ToList();
                query = query.Where(o => searchOrderIds.Contains(o.Id));
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Begins a database transaction to ensure atomicity of operations.
        /// </summary>
        /// <returns>DbContextTransaction object</returns>
        public DbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        /// <summary>
        /// Adds a new order to the database context.
        /// </summary>
        /// <param name="order">The order to add</param>
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        /// <summary>
        /// Adds a new order item to the database context.
        /// </summary>
        /// <param name="orderItem">The order item to add</param>
        public void AddOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
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
