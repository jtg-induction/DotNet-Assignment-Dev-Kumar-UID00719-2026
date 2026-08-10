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
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0,5)]
        public decimal Rating { get; set; }

        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
