using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class ChangePasswordRequestDto
    {
        [Key]
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}