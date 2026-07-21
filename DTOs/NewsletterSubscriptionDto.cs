using System.ComponentModel.DataAnnotations;

namespace timeify_rest.DTOs;

public class NewsletterSubscriptionDto
{
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;
}
