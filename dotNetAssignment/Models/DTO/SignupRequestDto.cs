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
        [RegularExpression(Regex.ValidNameRegex, ErrorMessage = ExceptionMessages.InvalidName)]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [RegularExpression(Regex.ValidEmailRegex, ErrorMessage = ExceptionMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(Regex.ValidPasswordRegex, ErrorMessage = ExceptionMessages.InvalidPassword)]
        public string Password { get; set; }

        [Required]
        [Phone]
        [RegularExpression(Regex.ValidPhoneNumberRegex, ErrorMessage = ExceptionMessages.InvalidPhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
