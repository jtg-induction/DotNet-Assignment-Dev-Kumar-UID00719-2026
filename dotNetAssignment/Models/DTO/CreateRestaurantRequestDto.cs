using System;
using System.ComponentModel.DataAnnotations;

public class CreateRestaurantRequestDto
{
    [Required]
    public Guid OwnerId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string AddressLineOne { get; set; }

    public string Landmark { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string Pincode { get; set; }
}