using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class CreateRestaurantResponseDto
    {
        /// <summary>
        /// The ID of the newly created restaurant.
        /// </summary>
        public Guid RestaurantId { get; set; }
    }
}