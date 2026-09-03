using dotNetAssignment.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderItemsRequestDto
    {
        /// <summary>
        /// The unique identifier of the product to be ordered.
        /// </summary>
        [Required]
        public Guid? Id { get; set; }

        /// <summary>
        /// The quantity of the product to be ordered.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = ExceptionMessages.QuantityGreaterThanOne)]
        public int? Quantity { get; set; }
    }
}
