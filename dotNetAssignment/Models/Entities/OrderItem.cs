using System;

namespace dotNetAssignment.Models.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        public Guid MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }

    }
}
