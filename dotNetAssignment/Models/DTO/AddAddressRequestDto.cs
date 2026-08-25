using System.ComponentModel.DataAnnotations;

using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.DTO.Address
{
    public class AddAddressRequestDto
    {
        [Required]
        [StringLength(255)]
        public string LineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        [Required]
        [RegularExpression(Regex.ValidPincodeRegex, ErrorMessage = ExceptionMessages.InvalidPincode)]
        public string Pincode { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }
    }
}
