using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Entities
{
    public class RestaurantOwners
    {
        [Key]
        [Column(Order = 0)]
        public Guid UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid RestaurantId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Users User { get; set; }
        public virtual Restaurants Restaurant { get; set; }
    }
}