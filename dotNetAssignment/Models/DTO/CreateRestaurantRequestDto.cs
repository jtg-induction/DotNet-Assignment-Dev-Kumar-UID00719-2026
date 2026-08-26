using dotNetAssignment.Constants;
using System;
using System.ComponentModel.DataAnnotations;

public class CreateRestaurantRequestDto
{
    /// <summary>
    /// The ID of the owner of the restaurant.
    /// </summary>
    [Required]
    public Guid OwnerId { get; set; }

    /// <summary>
    /// The name of the restaurant being created.
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string Name { get; set; }

    /// <summary>
    /// The address line one of the restaurant's location.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string AddressLineOne { get; set; }

    /// <summary>
    /// The landmark near the restaurant's location.
    /// </summary>
    [StringLength(255)]
    public string Landmark { get; set; }

    /// <summary>
    /// The city where the restaurant is located.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string City { get; set; }

    /// <summary>
    /// The state where the restaurant is located.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string State { get; set; }

    /// <summary>
    /// The pincode of the restaurant's location.
    /// </summary>
    [Required]
    [RegularExpression(Regex.ValidPincodeRegex, ErrorMessage = ExceptionMessages.InvalidPincode)]
    public string Pincode { get; set; }
}