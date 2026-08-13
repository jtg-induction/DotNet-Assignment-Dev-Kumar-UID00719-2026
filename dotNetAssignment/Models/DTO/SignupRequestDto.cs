using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Enums;

namespace dotNetAssignment.Models.DTO.SignUp
{
    public class SignupRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^[0-9]{10}$")]
        public string PhoneNumber { get; set; }
    }
}
