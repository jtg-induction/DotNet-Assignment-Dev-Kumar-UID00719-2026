using dotNetAssignment.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class ChangePasswordRequestDto
    {
        /// <summary>
        /// The old password of the user.
        /// </summary>
        [Required]
        [RegularExpression(Regex.ValidPasswordRegex, ErrorMessage = ExceptionMessages.InvalidPassword)]

        public string OldPassword { get; set; }

        /// <summary>
        /// The new password that the user wants to set.
        /// </summary>
        [Required]
        [StringLength(100)]
        [RegularExpression(Regex.ValidPasswordRegex, ErrorMessage = ExceptionMessages.InvalidPassword)]
        public string NewPassword { get; set; }
    }
}
