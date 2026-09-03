using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class AccessTokenRefreshResponse
    {
        /// <summary>
        /// New access token issued after a successful refresh operation.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
