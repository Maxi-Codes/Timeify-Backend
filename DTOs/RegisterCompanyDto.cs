using System.ComponentModel.DataAnnotations;

namespace timeify_rest.DTOs;

public class RegisterCompanyDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    public string? Street { get; set; }
    public int? HouseNumber { get; set; }
    public int? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}
