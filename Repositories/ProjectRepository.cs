using Microsoft.EntityFrameworkCore;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class ProjectRepository (AppDbContext appDbContext) : IProjectRepository
{
    public async Task<List<Project>> GetProjects()
    {
        var projects = await appDbContext.Projects.ToListAsync();
        return projects;
    }

    public async Task<Project> GetProject(Guid id)
    {
        var project = await appDbContext.Projects.FindAsync(id);
        return project;
    }

    public async Task<Project> CreateProjectAsync(CreateProjectDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new ArgumentException("CompanyId must not be empty.");
        }

        var companyExists = await appDbContext.Companies
            .AnyAsync(c => c.Id == dto.CompanyId);

        if (!companyExists)
        {
            throw new KeyNotFoundException(
                $"Company with ID '{dto.CompanyId}' was not found."
            );
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Name = dto.Name,
            Description = dto.Description,
            Address = new ProjectAddress
            {
                Street = dto.Address.Street,
                HouseNumber = dto.Address.HouseNumber,
                PostalCode = dto.Address.PostalCode,
                City = dto.Address.City,
                Country = dto.Address.Country,
                Latitude = dto.Address.Latitude,
                Longitude = dto.Address.Longitude
            },
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        appDbContext.Projects.Add(project);
        await appDbContext.SaveChangesAsync();

        return project;
    }

    public async Task<List<Project>> GetActiveProjects()
    {
        var projects = appDbContext.Projects.Where(p => p.IsActive).ToListAsync();
        return await projects;
    }

    public async Task<int> GetActiveProjectsCount()
    {
        var projectCount = appDbContext.Projects.CountAsync(p => p.IsActive);
        return await projectCount;
    }
}