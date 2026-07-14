using timeify_rest.DTOs;

namespace timeify_rest.Interfaces;

public interface IAuthRepository
{
    Task RegisterCompanyAsync(RegisterCompanyDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task RegisterUserAsync(RegisterUserDto dto);
}