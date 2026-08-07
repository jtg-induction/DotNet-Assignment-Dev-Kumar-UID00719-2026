using System;
using System.ComponentModel.DataAnnotations;

namespace dotNetAssignment.Models.Entities
{
    public class RefreshTokens
    {
        [Key]
       public Guid JwtId { get; set; }
    }
}