using Microsoft.EntityFrameworkCore;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Enums;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class UserRepository(AppDbContext appDbContext) : IUserRepository
{
    public async Task<List<UserResponseDto>> GetUsersAsync(
        Guid? companyId = null,
        Role? role = null)
    {
        var query = appDbContext.Users
            .AsNoTracking()
            .AsQueryable();

        if (companyId.HasValue)
            query = query.Where(u => u.CompanyId == companyId.Value);

        if (role.HasValue)
            query = query.Where(u => u.Role == role.Value);

        return await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                CompanyId = u.CompanyId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }

    public async Task<UserResponseDto?> GetUserAsync(Guid id)
    {
        return await appDbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                CompanyId = u.CompanyId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await appDbContext.Users.FindAsync(id);

        if (user is null)
            return null;

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailExists = await appDbContext.Users
            .AnyAsync(u => u.Id != id && u.Email.ToLower() == normalizedEmail);

        if (emailExists)
            throw new InvalidOperationException("A user with this email address already exists.");

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = normalizedEmail;

        await appDbContext.SaveChangesAsync();
        return ToResponseDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await appDbContext.Users.FindAsync(id);

        if (user is null)
            return false;

        appDbContext.Users.Remove(user);
        await appDbContext.SaveChangesAsync();
        return true;
    }

    private static UserResponseDto ToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }
}
