using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.Entities
{
    public class Restaurant
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
        [RegularExpression(Regex.ValidPincodeRegex)]
        public string Pincode { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }

    }
}
