using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using dotNetAssignment.Models.Enums;

namespace dotNetAssignment.Models.Entities
{
    public class Users
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100), MinLength(2)]
        public string Name { get; set; }

        [Required]
        public UserRole Role { get; set; }
        
        [Required]
        [EmailAddress]
        [Index("IX_Email", IsUnique = true)]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100), MinLength(8)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public decimal Balance { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<UserAddresses> UserAddresses { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<RestaurantOwners> RestaurantOwners { get; set; }

    }
}