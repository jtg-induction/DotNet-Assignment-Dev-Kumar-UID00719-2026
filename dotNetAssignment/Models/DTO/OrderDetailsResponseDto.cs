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
        public string OrderStatus { get; set; }
        public string RestaurantName { get; set; }
        public OrderAddressResponseDto DeliveryAddress { get; set; }
        public List<OrderItemDetailsDto> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
