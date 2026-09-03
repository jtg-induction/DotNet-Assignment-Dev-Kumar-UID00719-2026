using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.Enums
{
    public enum OrderStatus
    {
        Placed = 1,
        Rejected = 2,
        Accepted = 3,
        Dispatched = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
