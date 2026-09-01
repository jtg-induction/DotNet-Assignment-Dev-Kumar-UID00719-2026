using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OnboardNewRestaurantOwnerDto
    {
        /// <summary>
        /// The ID of the restaurant to be onboarded.
        /// </summary>
        [Required]
        public Guid? RestaurantId { get; set; }

        /// <summary>
        /// The ID of the owner to be onboarded.
        /// </summary>
        [Required]
        public Guid? OwnerId { get; set; }
    }
}
