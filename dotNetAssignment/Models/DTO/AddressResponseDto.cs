using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO.Address
{
    public class AddressResponseDto
    {
        /// <summary>
        /// The unique identifier of the address.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The first line of the address.
        /// </summary>
        public string AddressLineOne { get; set; }

        /// <summary>
        /// The landmark of the address.
        /// </summary>
        public string Landmark { get; set; }

        /// <summary>
        /// The pincode of the address.
        /// </summary>
        public string Pincode { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The state of the address.
        /// </summary>
        public string State { get; set; }
    }
}
