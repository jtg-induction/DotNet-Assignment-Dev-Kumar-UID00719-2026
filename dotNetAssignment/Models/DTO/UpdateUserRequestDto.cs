using dotNetAssignment.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class UpdateUserRequestDto
    {
        /// <summary>
        /// The new name of the user.
        /// </summary>
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        /// <summary>
        /// The new email address of the user.
        /// </summary>
        [RegularExpression(Regex.ValidPhoneNumberRegex, ErrorMessage = ExceptionMessages.InvalidPhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
