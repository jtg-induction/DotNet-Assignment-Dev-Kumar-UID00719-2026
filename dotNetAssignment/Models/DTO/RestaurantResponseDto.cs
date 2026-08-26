using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.DTO
{
    public class RestaurantResponseDto
    {
        /// <summary>
        /// The unique identifier of the restaurant.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the restaurant.
        /// </summary>
        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        /// <summary>
        /// The first line of the restaurant's address.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string AddressLineOne { get; set; }

        /// <summary>
        /// The rating of the restaurant.
        /// </summary>
        [Range(0, 5)]
        public decimal Rating { get; set; }

        /// <summary>
        /// The pincode of the restaurant's address.
        /// </summary>
        [Required]
        [RegularExpression(Regex.ValidPincodeRegex)]
        public string Pincode { get; set; }

        /// <summary>
        /// The state of the restaurant's address.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string State { get; set; }

        /// <summary>
        /// The city of the restaurant's address.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>
        /// The landmark of the restaurant's address.
        /// </summary>
        [StringLength(255)]
        public string Landmark { get; set; }

    }
}
