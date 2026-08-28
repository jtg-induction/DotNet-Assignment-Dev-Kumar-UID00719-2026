using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class UpdateOrderStatusDto
    {
        /// <summary>
        /// The unique identifier of the order to be updated.
        /// </summary>
        [Required]
        public Guid OrderId { get; set; }

        /// <summary>
        /// The new status of the order.
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; }
    }
}
