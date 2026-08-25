using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class UpdateUserRequestDto
    {
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }


        [Phone]
        public string PhoneNumber { get; set; }
    }
}
