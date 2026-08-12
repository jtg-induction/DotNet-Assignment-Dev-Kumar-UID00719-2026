using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderItemsRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}