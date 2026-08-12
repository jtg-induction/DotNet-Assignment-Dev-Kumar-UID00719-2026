using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class MenuResponseDto
    {
        public Guid Id { get; set; }

        public string DishName { get; set; }

        public decimal Price { get; set; }

        public decimal Rating { get; set; }
    }
}