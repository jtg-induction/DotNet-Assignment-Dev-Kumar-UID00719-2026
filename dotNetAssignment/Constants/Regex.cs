using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Constants
{
    public class Regex
    {
        public const string validPincodeRegex = @"^[1-9][0-9]{5}$";
        public const string validEmailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public const string validPhoneNumberRegex = @"^[0-9]{10}$";
    }
}
