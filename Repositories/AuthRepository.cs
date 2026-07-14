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
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = dto.CompanyName,
            Street = dto.Street,
            HouseNumber = dto.HouseNumber,
            PostalCode = dto.PostalCode,
            City = dto.City,
            Country = dto.Country,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Email = dto.AdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Role.PlatformAdmin,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };

        db.Companies.Add(company);
        db.Users.Add(user);

        await db.SaveChangesAsync();
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            throw new Exception("Invalid credentials");

        var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isValid)
            throw new Exception("Invalid credentials");

        var token = jwt.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token
        };
    }

    public async Task RegisterUserAsync(RegisterUserDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = (Role)dto.Role
        };
        
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}