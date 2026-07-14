using timeify_rest.DTOs;
using timeify_rest.Entities;

namespace timeify_rest.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetProjects();
    Task<Project> GetProject(Guid id);
    Task<Project> CreateProjectAsync(CreateProjectDto dto);
    Task<List<Project>> GetActiveProjects();
    Task<int> GetActiveProjectsCount();
}