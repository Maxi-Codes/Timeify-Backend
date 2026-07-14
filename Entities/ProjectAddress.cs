namespace timeify_rest.Entities;

public class ProjectAddress
{
    public string Street { get; set; } = string.Empty;
    public string? HouseNumber { get; set; }

    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = "DE";

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
