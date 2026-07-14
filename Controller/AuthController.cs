using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthRepository auth) : ControllerBase
{
    [HttpPost("register-company")]
    public async Task<IActionResult> RegisterCompany(RegisterCompanyDto dto)
    {
        await auth.RegisterCompanyAsync(dto);
        return Ok("Company created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await auth.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
    {
        await auth.RegisterUserAsync(dto);
        return Ok(dto);
    }
}