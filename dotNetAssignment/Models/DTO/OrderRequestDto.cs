using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderRequestDto
    {
        /// <summary>
        /// The unique identifier of the address where the order should be delivered.
        /// </summary>
        [Required]
        public Guid AddressId { get; set; }

        /// <summary>
        /// The list of order items included in the order.
        /// </summary>
        [Required]
        public List<OrderItemsRequestDto> OrderItems { get; set; }
    }
}
