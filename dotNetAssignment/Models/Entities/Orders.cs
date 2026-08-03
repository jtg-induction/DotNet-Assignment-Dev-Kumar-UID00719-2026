using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Entities
{
    public class Orders
    {
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("Restaurant")]
        public Guid RestaurantId { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime PlacedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [StringLength(255)]
        public string AddressLineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        public string Pincode { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        public virtual Users User { get; set; }
        public virtual Restaurants Restaurant { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}