using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using dotNetAssignment.Models.DTO.Address;
using dotNetAssignment.Models.Enums;

using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.DTO.SignUp
{
    public class SignupRequestDto
    {
        [Required]
        [RegularExpression(Regex.validNameRegex)]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [RegularExpression(Regex.validEmailRegex, ErrorMessage = ExceptionMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(Regex.validPasswordRegex)]
        public string Password { get; set; }

        [Required]
        [Phone]
        [RegularExpression(Regex.validPhoneNumberRegex, ErrorMessage = ExceptionMessages.InvalidPhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
