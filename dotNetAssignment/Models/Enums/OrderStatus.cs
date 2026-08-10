using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Enums
{
    public enum OrderStatus
    {
        Placed = 1,
        Accepted = 2,
        Rejected = 3,
        Dispatched = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
