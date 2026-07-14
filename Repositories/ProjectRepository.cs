using Microsoft.EntityFrameworkCore;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class ProjectRepository(AppDbContext appDbContext) : IProjectRepository
{
    public async Task<List<Project>> GetProjectsAsync(Guid? companyId = null, bool? isActive = null)
    {
        var query = appDbContext.Projects
            .AsNoTracking()
            .AsQueryable();

        if (companyId.HasValue)
            query = query.Where(p => p.CompanyId == companyId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        return await query
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectAsync(Guid id)
    {
        return await appDbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
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
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Address = CreateAddress(dto.Address),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        appDbContext.Projects.Add(project);
        await appDbContext.SaveChangesAsync();

        return project;
    }

    public async Task<Project?> UpdateProjectAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await appDbContext.Projects.FindAsync(id);

        if (project is null)
            return null;

        project.Name = dto.Name.Trim();
        project.Description = dto.Description?.Trim();
        project.Address = CreateAddress(dto.Address);
        project.UpdatedAt = DateTime.UtcNow;

        await appDbContext.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> UpdateProjectStatusAsync(Guid id, bool isActive)
    {
        var project = await appDbContext.Projects.FindAsync(id);

        if (project is null)
            return null;

        project.IsActive = isActive;
        project.UpdatedAt = DateTime.UtcNow;

        await appDbContext.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = await appDbContext.Projects.FindAsync(id);

        if (project is null)
            return false;

        var hasTimeEntries = await appDbContext.TimeEntries.AnyAsync(t => t.ProjectId == id);

        if (hasTimeEntries)
        {
            throw new InvalidOperationException(
                "The project cannot be deleted because time entries reference it. Deactivate it instead.");
        }

        appDbContext.Projects.Remove(project);
        await appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetActiveProjectsCountAsync(Guid? companyId = null)
    {
        var query = appDbContext.Projects.Where(p => p.IsActive);

        if (companyId.HasValue)
            query = query.Where(p => p.CompanyId == companyId.Value);

        return await query.CountAsync();
    }

    private static ProjectAddress CreateAddress(ProjectAddressDto dto)
    {
        return new ProjectAddress
        {
            Street = dto.Street.Trim(),
            HouseNumber = dto.HouseNumber?.Trim(),
            PostalCode = dto.PostalCode.Trim(),
            City = dto.City.Trim(),
            Country = dto.Country.Trim(),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }
}
