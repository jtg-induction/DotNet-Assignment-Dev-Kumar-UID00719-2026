using System;
using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid JwtId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
