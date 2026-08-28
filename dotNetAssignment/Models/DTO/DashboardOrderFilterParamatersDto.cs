using dotNetAssignment.Models.Enums;
using System;

namespace dotNetAssignment.Models.DTO
{
    public class DashboardOrderFilterParamatersDto
    {
        /// <summary>
        /// Status used to filter the order.
        /// </summary>
        public OrderStatus? Status {  get; set; }

        /// <summary>
        /// Date when the order was placed, used to filter the order.
        /// </summary>
        public DateTime? PlacedAt { get; set; }
    }
}
