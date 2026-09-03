using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderItemDetailsDto
    {
        /// <summary>
        /// The name of the dish in the order item.
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// The quantity of the dish ordered.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The price of the dish in the order item.
        /// </summary>
        public decimal Price { get; set; }
    }
}
