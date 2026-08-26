using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderDetailsResponseDto
    {
        /// <summary>
        /// The order status of the order.
        /// </summary>
        public string OrderStatus { get; set; }
       
        /// <summary>
        /// The name of the restaurant.
        /// </summary>
        public string RestaurantName { get; set; }
       
        /// <summary>
        /// The delivery address for the order.
        /// </summary>
        public OrderAddressResponseDto DeliveryAddress { get; set; }
        
        /// <summary>
        /// The list of items in the order.
        /// </summary>
        public List<OrderItemDetailsDto> OrderItems { get; set; }
        
        /// <summary>
        /// The total amount of the order.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
