using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class TopTenMostOrderedRequestDto
    {
        /// <summary>
        /// Unique identifier for order items to exclude from the report. This can be a comma-separated list of item IDs.
        /// </summary>
        public string ExcludeItemIds { get; set; }
    }
}
