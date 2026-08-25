using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class DeactivateAccountRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
