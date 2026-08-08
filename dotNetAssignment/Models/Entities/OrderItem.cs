using System;

namespace dotNetAssignment.Models.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid MenuId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Menu Menu { get; set; }
    }
}
