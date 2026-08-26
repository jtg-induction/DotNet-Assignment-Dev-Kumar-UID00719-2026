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
        /// <summary>
        /// The email address of the user attempting to log in.
        /// </summary>
        [Required]
        [StringLength(255)]
        [RegularExpression(Regex.ValidEmailRegex, ErrorMessage = ExceptionMessages.InvalidEmail)]
        public string Email { get; set; }

        /// <summary>
        /// The password of the user attempting to log in.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
