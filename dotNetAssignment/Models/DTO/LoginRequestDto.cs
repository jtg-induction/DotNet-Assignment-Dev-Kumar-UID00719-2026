using dotNetAssignment.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO.Login
{
    public class LoginRequestDto
    {
        [Required]
        [StringLength(255)]
        [RegularExpression(Regex.ValidEmailRegex, ErrorMessage = ExceptionMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
