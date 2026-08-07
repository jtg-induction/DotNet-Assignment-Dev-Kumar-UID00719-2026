using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO.Address
{
    public class AddressRequestDto
    {
        [Required]
        [StringLength(255)]
        public string LineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        [Required]
        public string Pincode { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }
    }
}