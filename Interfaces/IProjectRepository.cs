using timeify_rest.DTOs;
using timeify_rest.Entities;

namespace timeify_rest.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetProjectsAsync(Guid? companyId = null, bool? isActive = null);
    Task<Project?> GetProjectAsync(Guid id);
    Task<Project> CreateProjectAsync(CreateProjectDto dto);
    Task<Project?> UpdateProjectAsync(Guid id, UpdateProjectDto dto);
    Task<Project?> UpdateProjectStatusAsync(Guid id, bool isActive);
    Task<bool> DeleteProjectAsync(Guid id);
    Task<int> GetActiveProjectsCountAsync(Guid? companyId = null);
}
