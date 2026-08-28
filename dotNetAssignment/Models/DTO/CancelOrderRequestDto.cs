using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class CancelOrderRequestDto
    {
        /// <summary>
        /// The unique identifier of the order to be canceled.
        /// </summary>
        [Required]
        public Guid OrderId { get; set; }
    }
}
