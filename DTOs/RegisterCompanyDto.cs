namespace timeify_rest.DTOs;

public class RegisterCompanyDto
{
    public string CompanyName { get; set; } = null!;
    public string? Street { get; set; }
    public int? HouseNumber { get; set; }
    public int? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string AdminEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; }
    public string LastName { get; set; }
}