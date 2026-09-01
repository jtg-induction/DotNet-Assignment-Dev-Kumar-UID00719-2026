using System;
using System.ComponentModel.DataAnnotations;
using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.DTO.Address
{
    public class UpdateAddressRequestDto
    {
        /// <summary>
        /// The unique identifier of the address to be updated.
        /// </summary>
        [Required]
        public Guid? AddressId { get; set; }

        /// <summary>
        /// The first line of the address.
        /// </summary>
        [StringLength(255)]
        public string LineOne { get; set; }

        /// <summary>
        /// The landmark associated with the address.
        /// </summary>
        [StringLength(255)]
        public string Landmark { get; set; }

        /// <summary>
        /// The pincode of the address.
        /// </summary>
        [RegularExpression(Regex.ValidPincodeRegex, ErrorMessage = ExceptionMessages.InvalidPincode)]
        public string Pincode { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>
        /// The state of the address.
        /// </summary>
        [StringLength(100)]
        public string State { get; set; }
    }
}
