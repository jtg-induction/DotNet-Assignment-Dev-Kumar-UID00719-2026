using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class PlaceOrderResponseDto
    {
        /// <summary>
        /// The unique identifier of the placed order.
        /// </summary>
        public Guid OrderId { get; set; }
    }
}
