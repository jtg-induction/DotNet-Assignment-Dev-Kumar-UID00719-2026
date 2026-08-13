using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using dotNetAssignment.Models.Enums;
using dotNetAssignment.Constants;

namespace dotNetAssignment.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        [RegularExpression(Regex.validNameRegex)]
        public string Name { get; set; }

        [Required]
        public UserRole Role { get; set; }
        
        [Required]
        [Index("IX_Email", IsUnique = true)]
        [StringLength(255)]
        [RegularExpression(Regex.validEmailRegex)]
        public string Email { get; set; }

        [Required]
        [StringLength(100), MinLength(8)]
        [RegularExpression(Regex.validPasswordRegex)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Balance { get; set; }

        [Required]
        [Index("IX_PhoneNumber", IsUnique = true)]
        [RegularExpression(Regex.validPhoneNumberRegex)]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }

    }
}
