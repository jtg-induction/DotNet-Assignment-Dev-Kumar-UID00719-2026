using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderResponseDto
    {
        /// <summary>
        /// Unique identifier of the order.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Name of the restaurant to which the order belongs.
        /// </summary>
        public string RestaurantName { get; set; }

        /// <summary>
        /// Name of the customer who placed the order.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Date when the order was placed.
        /// </summary>
        public DateTime PlacedAt { get; set; }

        /// <summary>
        /// Current status of the order.
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
    }
}
