using System.ComponentModel.DataAnnotations;

using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.DTO.Address
{
    public class AddAddressRequestDto
    {
        /// <summary>
        /// The first line of the address.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string LineOne { get; set; }

        /// <summary>
        /// The Landmark of the address (optional).
        /// </summary>
        [StringLength(255)]
        public string Landmark { get; set; }

        /// <summary>
        /// The pincode of the address
        /// </summary>
        [Required]
        [RegularExpression(Regex.ValidPincodeRegex, ErrorMessage = ExceptionMessages.InvalidPincode)]
        public string Pincode { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>
        /// The state of the address.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string State { get; set; }
    }
}
