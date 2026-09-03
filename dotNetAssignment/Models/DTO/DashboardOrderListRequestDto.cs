using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class DashboardOrderListRequestDto
    {
        /// <summary>
        /// Page number to retrieve.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of orders to retrieve per page.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Order id used to search order.
        /// </summary>
        public string SearchOrderIds { get; set; } 

        /// <summary>
        /// Fields by which orders are to be sorted.
        /// </summary>
        public SortOrdersFields SortBy { get; set; }

        /// <summary>
        /// Sort direction.
        /// </summary>
        public string SortOrder { get; set; } = "desc";

        /// <summary>
        /// Status used to filter the order.
        /// </summary>
        public OrderStatus? Status { get; set; }

        /// <summary>
        /// Start date used to filter the order.
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// End date used to filter the order.
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}
