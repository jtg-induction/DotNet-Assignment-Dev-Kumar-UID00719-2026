using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class MenuListResponseDto
    {
        /// <summary>
        /// List of menu items.
        /// </summary>
        public List<MenuResponseDto> Menu { get; set; }

        /// <summary>
        /// Current page number in the paginated response.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page in the paginated response.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items in the response.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages in the paginated response.
        /// </summary>
        public int TotalPages { get; set; }
    }
}
