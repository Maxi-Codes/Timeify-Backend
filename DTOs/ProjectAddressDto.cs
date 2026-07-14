using System.ComponentModel.DataAnnotations;

namespace timeify_rest.DTOs;

public class ProjectAddressDto
{
    [Required]
    public string Street { get; set; } = string.Empty;

    public string? HouseNumber { get; set; }

    [Required]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = "DE";

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }
}
