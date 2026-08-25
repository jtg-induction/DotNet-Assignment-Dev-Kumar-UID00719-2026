using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Constants
{
    public class ExceptionMessages
    {
        public const string UserAlreadyExists = "User already exists";
        public const string EmailOrPhoneNumberAlreadyExists = "Email or phone number already exists";
        public const string UserNotFound = "User not found";
        public const string UserInactive = "User is inactive";
        public const string InvalidEmailOrPassword = "Invalid email or password";
        public const string InvalidName = "Invalid name";
        public const string InvalidEmail = "Invalid email";
        public const string InvalidPassword = "Invalid password";
        public const string InvalidRequest = "Invalid request";
        public const string InvalidPhoneNumber = "Invalid phone number";
        public const string InvalidPincode = "Invalid pincode";
        public const string InvalidRefreshToken = "Invalid refresh token";
        public const string AddressNotFound = "Address not found";
        public const string WrongPassword = "Wrong password";
        public const string SamePassword = "New password can not be same as old password";
        public const string SamePhoneNumber = "New phone number can not be same as old phone number";
        public const string UserNotUpdated = "User not updated";
    }
}
