using System.ComponentModel.DataAnnotations;

namespace timeify_rest.DTOs;

public class UpdateProjectStatusDto
{
    [Required]
    public bool? IsActive { get; set; }
}
