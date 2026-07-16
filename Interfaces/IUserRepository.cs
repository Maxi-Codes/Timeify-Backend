using timeify_rest.DTOs;
using timeify_rest.Enums;

namespace timeify_rest.Interfaces;

public interface IUserRepository
{
    Task<List<UserResponseDto>> GetUsersAsync(Guid? companyId = null, Role? role = null);
    Task<UserResponseDto?> GetUserAsync(Guid id);
    Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid id);
}
