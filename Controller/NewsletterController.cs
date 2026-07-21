using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/newsletter")]
public class NewsletterController(INewsletterRepository newsletterRepository) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(NewsletterSubscriptionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Email is required.");

        var created = await newsletterRepository.SubscribeAsync(dto.Email);

        if (!created)
        {
            return Ok(new
            {
                message = "This email address is already subscribed to the newsletter."
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Newsletter subscription created successfully."
        });
    }
}
