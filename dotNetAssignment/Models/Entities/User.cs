using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using dotNetAssignment.Models.Enums;

namespace dotNetAssignment.Models.Entities
{
    public class User
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
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        public string Email { get; set; }

        [Required]
        [StringLength(100), MinLength(8)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Balance { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^[0-9]{10}$")]
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }

    }
}
