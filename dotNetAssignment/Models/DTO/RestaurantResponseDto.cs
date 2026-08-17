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
        public Guid Id { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string AddressLineOne { get; set; }

        [Range(0, 5)]
        public decimal Rating { get; set; }

        [Required]
        [RegularExpression(Regex.validPincodeRegex)]
        public string Pincode { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

    }
}
