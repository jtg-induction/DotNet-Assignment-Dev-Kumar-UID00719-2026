using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetAssignment.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime PlacedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        [StringLength(255)]
        public string AddressLineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]{5}$")]
        public string Pincode { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
