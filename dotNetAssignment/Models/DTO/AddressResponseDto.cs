using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO.Address
{
    public class AddressResponseDto
    {
        public Guid Id { get; set; }

        public string LineOne { get; set; }

        public string Landmark { get; set; }

        public string Pincode { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}
