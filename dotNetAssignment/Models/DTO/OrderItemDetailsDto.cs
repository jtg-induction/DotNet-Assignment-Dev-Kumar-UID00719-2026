using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderItemDetailsDto
    {
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
