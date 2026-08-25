using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO.SignUp
{
    public class SignupRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [RegularExpression(Regex.ValidEmailRegex)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Phone]
        [RegularExpression(Regex.ValidPhoneNumberRegex)]
        public string PhoneNumber { get; set; }
    }
}
