using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Enums;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthRepository authRepository) : ControllerBase
{
    [HttpPost("register-company")]
    public async Task<IActionResult> RegisterCompany(RegisterCompanyDto dto)
    {
        var validationResult = ValidateRegistration(
            dto.AdminEmail,
            dto.Password,
            dto.FirstName,
            dto.LastName);

        if (validationResult is not null)
            return validationResult;

        if (string.IsNullOrWhiteSpace(dto.CompanyName))
            return BadRequest("Company name is required.");

        try
        {
            await authRepository.RegisterCompanyAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Company and administrator account created successfully."
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email and password are required.");

        var result = await authRepository.LoginAsync(dto);

        if (result is null)
            return Unauthorized("Invalid email or password.");

        return Ok(result);
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
            return BadRequest("CompanyId must not be empty.");

        if (!dto.Role.HasValue || !Enum.IsDefined(dto.Role.Value))
            return BadRequest("The user role is invalid.");

        if (dto.Role.Value == Role.PlatformAdmin)
            return BadRequest("PlatformAdmin cannot be assigned through this endpoint.");

        var validationResult = ValidateRegistration(
            dto.Email,
            dto.Password,
            dto.FirstName,
            dto.LastName);

        if (validationResult is not null)
            return validationResult;

        try
        {
            await authRepository.RegisterUserAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "User created successfully."
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    private BadRequestObjectResult? ValidateRegistration(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return BadRequest("Password must contain at least 8 characters.");

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return BadRequest("First name and last name are required.");

        return null;
    }
}
