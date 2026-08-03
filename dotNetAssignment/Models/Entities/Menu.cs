using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Entities
{
    public class Menu
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DishName { get; set; }
        
        [Required]
        public decimal Price { get; set; }

        public int Rating { get; set; }

        [ForeignKey("Restaurant")]
        public Guid RestaurantId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual Restaurants Restaurant { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}