using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderRequestDto
    {
        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid AddressId { get; set; }

        [Required]
        public List<OrderItemsRequestDto> orderItems { get; set; }
    }
}
