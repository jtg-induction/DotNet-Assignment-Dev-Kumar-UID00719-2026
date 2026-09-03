using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dotNetAssignment.Constants
{
    public class ExceptionMessages
    {
        public const string UserAlreadyExists = "User already exists";
        public const string PhoneNumberAlreadyExists = "Phone number already exists";
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
        public const string RestaurantDoesntExists = "Restaurant doesn't exist";
        public const string OrderDoesntExists = "Order doesn't exist";
        public const string MenuItemDoesntExists = "Item doesn't exists";
        public const string InvalidOrderItems = "Invalid order items";
        public const string InsufficientBalance = "Insufficient Balance";
        public const string InsufficientStock = "Insufficient Stock";
        public const string AddressNotFound = "Address not found";
        public const string WrongPassword = "Wrong password";
        public const string SamePassword = "New password can not be same as old password";
        public const string OrderCannotBeCancelled = "Order cannot be cancelled";
        public const string PageNotFound = "Page not found";
        public const string SamePhoneNumber = "New phone number can not be same as old phone number";
        public const string UserNotUpdated = "User not updated";
        public const string AddressNotUpdated = "Address not updated";
        public const string OrderStatusCanNotBeUpdated = "Order status can not be updated";
        public const string OrderStatusCanNotBeSame = "New order status can not be same as old";
        public const string OrderDoesNotExist = "Order not found";
        public const string YouCantPerformThisAction = "You can not perform this action";
        public const string InvalidValue = "Invalid value for";
        public const string QuantityGreaterThanOne = "Quantity should be atleast one";
        public const string SameName = "New name can not be same as old name";
        public const string AvailableQuantityIs = "Available quanity for this item is ";
        public const string OrderAlreadyCancelled = "Order is already cancelled";
    }
}
