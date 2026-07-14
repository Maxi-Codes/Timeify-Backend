using Microsoft.EntityFrameworkCore;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Enums;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class AuthRepository(AppDbContext db, IJwtService jwt) : IAuthRepository
{
    public async Task RegisterCompanyAsync(RegisterCompanyDto dto)
    {
        var normalizedEmail = dto.AdminEmail.Trim().ToLowerInvariant();
        var emailExists = await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);

        if (emailExists)
            throw new InvalidOperationException("A user with this email address already exists.");

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = dto.CompanyName.Trim(),
            Street = dto.Street?.Trim(),
            HouseNumber = dto.HouseNumber,
            PostalCode = dto.PostalCode,
            City = dto.City?.Trim(),
            Country = dto.Country?.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Role.CompanyOwner,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
        };

        db.Companies.Add(company);
        db.Users.Add(user);

        await db.SaveChangesAsync();
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail);

        if (user is null)
            return null;

        var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isValid)
            return null;

        var token = jwt.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token
        };
    }

    public async Task RegisterUserAsync(RegisterUserDto dto)
    {
        var company = await db.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CompanyId);

        if (company is null)
            throw new KeyNotFoundException($"Company with ID '{dto.CompanyId}' was not found.");

        if (!company.IsActive)
            throw new InvalidOperationException("Users cannot be added to an inactive company.");

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailExists = await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);

        if (emailExists)
            throw new InvalidOperationException("A user with this email address already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Email = normalizedEmail,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role!.Value
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
