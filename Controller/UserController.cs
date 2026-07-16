using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Enums;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/users")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll(
        [FromQuery] Guid? companyId,
        [FromQuery] Role? role)
    {
        if (companyId == Guid.Empty)
            return BadRequest("CompanyId must not be empty.");

        if (role.HasValue && !Enum.IsDefined(role.Value))
            return BadRequest("The user role is invalid.");

        var users = await userRepository.GetUsersAsync(companyId, role);
        return Ok(users);
    }

    [HttpGet("{id:guid}", Name = nameof(GetUserById))]
    public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("User ID must not be empty.");

        var user = await userRepository.GetUserAsync(id);

        if (user is null)
            return NotFound($"User with ID '{id}' was not found.");

        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, UpdateUserDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("User ID must not be empty.");

        var validationResult = ValidateUser(dto);
        if (validationResult is not null)
            return validationResult;

        try
        {
            var user = await userRepository.UpdateUserAsync(id, dto);

            if (user is null)
                return NotFound($"User with ID '{id}' was not found.");

            return Ok(user);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("User ID must not be empty.");

        var deleted = await userRepository.DeleteUserAsync(id);

        if (!deleted)
            return NotFound($"User with ID '{id}' was not found.");

        return NoContent();
    }

    private BadRequestObjectResult? ValidateUser(UpdateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest("First name and last name are required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Email is required.");

        return null;
    }
}
