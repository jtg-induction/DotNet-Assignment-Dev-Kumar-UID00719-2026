using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Constants
{
    public class ExceptionMessages
    {
        public const string UserAlreadyExists = "User already exists";
        public const string UserNotFound = "User not found";
        public const string InvalidEmailOrPassword = "Invalid email or password";
        public const string InvalidEmail = "Invalid email";
        public const string InvalidRequest = "Invalid request";
        public const string InvalidPhoneNumber = "Invalid phone number";
        public const string InvalidPincode = "Invalid pincode";
        public const string InvalidRefreshToken = "Invalid refresh token";
        public const string RestaurantDoesntExists = "Restaurant doesn't exists";
        public const string OrderDoesntExists = "Order doesn't exist";
        public const string MenuItemDoesntExists = "Item doesn't exists";
        public const string InsufficientBalance = "Insufficient Balance";
        public const string InsufficientStock = "Insufficient Stock";
        public const string AddressNotFound = "Address not found";
        public const string WrongPassword = "Wrong password";
        public const string SamePassword = "New password can not be same as old password";
        public const string OrderCannotBeCancelled = "Order cannot be cancelled";
        public const string PageNotFound = "Page not found";
    }
}
