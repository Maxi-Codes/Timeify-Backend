namespace timeify_rest.DTOs;

public class CreateProjectDto
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectAddressDto Address { get; set; } = new();
}