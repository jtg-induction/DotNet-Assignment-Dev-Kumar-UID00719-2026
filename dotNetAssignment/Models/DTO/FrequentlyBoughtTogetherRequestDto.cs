using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class FrequentlyBoughtTogetherRequestDto
    {
        /// <summary>
        /// Unique identifier of the restaurant for which to generate the report.
        /// </summary>
        [Required]
        public string RestaurantId { get; set; }
    }
}
