using System;
using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.DTO.Address
{
    public class UpdateAddressRequestDto
    {
        public Guid AddressId { get; set; }

        [StringLength(255)]
        public string LineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        public string Pincode { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }
    }
}
