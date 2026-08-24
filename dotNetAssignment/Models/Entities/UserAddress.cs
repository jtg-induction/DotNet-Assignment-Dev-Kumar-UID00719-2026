using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Entities
{
    public class UserAddress
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [StringLength(255)]
        public string LineOne { get; set; }

        [StringLength(255)]
        public string Landmark { get; set; }

        [Required]
        [RegularExpression(@"^[1-9][0-9]{5}$")]
        public string Pincode { get; set; }
        
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        
        [Required]
        [StringLength(100)]
        public string State { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
