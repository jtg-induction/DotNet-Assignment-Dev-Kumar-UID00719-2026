using System;
using System.ComponentModel.DataAnnotations;
using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.DTO.Address
{
    public class UpdateAddressRequestDto
    {
        [Required]
        public Guid AddressId { get; set; }

        [StringLength(255)]
        public string LineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        [RegularExpression(Regex.ValidPincodeRegex, ErrorMessage = ExceptionMessages.InvalidPincode)]
        public string Pincode { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }
    }
}
