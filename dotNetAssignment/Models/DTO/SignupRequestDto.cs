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
        /// <summary>
        /// The name of the user signing up.
        /// </summary>
        [Required]
        [RegularExpression(Regex.ValidNameRegex, ErrorMessage = ExceptionMessages.InvalidName)]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        /// <summary>
        /// The email address of the user signing up.
        /// </summary>
        [Required]
        [StringLength(255)]
        [RegularExpression(Regex.ValidEmailRegex, ErrorMessage = ExceptionMessages.InvalidEmail)]
        public string Email { get; set; }

        /// <summary>
        /// The password of the user signing up.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(Regex.ValidPasswordRegex, ErrorMessage = ExceptionMessages.InvalidPassword)]
        public string Password { get; set; }

        /// <summary>
        /// The phone number of the user signing up.
        /// </summary>
        [Required]
        [Phone]
        [RegularExpression(Regex.ValidPhoneNumberRegex, ErrorMessage = ExceptionMessages.InvalidPhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
