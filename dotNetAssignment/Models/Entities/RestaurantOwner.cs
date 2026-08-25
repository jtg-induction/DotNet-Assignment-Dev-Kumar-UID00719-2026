using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNetAssignment.Models.Entities
{
    public class RestaurantOwner
    {
        [Key]
        [Column(Order = 0)]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
