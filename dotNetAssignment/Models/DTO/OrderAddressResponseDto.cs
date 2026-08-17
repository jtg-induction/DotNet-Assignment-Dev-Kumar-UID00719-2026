using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Models.DTO
{
    public class OrderAddressResponseDto
    {
        public string AddressLineOne { get; set; }
        public string Landmark { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
