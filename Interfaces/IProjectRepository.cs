using timeify_rest.Entities;

namespace timeify_rest.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetActiveProjects();
    Task<int> GetActiveProjectsCount();
}