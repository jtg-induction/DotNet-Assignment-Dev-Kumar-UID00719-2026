using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class MenuResponseDto
    {
        /// <summary>
        /// The unique identifier for the menu item.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the dish in the menu.
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// The price of the dish.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The rating of the dish.
        /// </summary>
        public decimal Rating { get; set; }
    }
}
