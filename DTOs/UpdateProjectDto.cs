using System.ComponentModel.DataAnnotations;

namespace timeify_rest.DTOs;

public class UpdateProjectDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public ProjectAddressDto Address { get; set; } = new();
}
