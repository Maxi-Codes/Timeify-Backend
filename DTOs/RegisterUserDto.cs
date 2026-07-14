using System.ComponentModel.DataAnnotations;
using timeify_rest.Enums;

namespace timeify_rest.DTOs;

public class RegisterUserDto
{
    public Guid CompanyId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public Role? Role { get; set; }
}
