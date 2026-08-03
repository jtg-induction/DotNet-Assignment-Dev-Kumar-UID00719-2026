using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Entities
{
    public class UserAddresses
    {
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

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

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual Users User { get; set; }
    }
}