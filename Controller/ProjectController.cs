using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/projects")]
public class ProjectController (IProjectRepository projectRepository) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await projectRepository.GetActiveProjects();

        if (projects.Count == 0) return NotFound();
        
        return Ok(projects);
    }
    
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public async Task<ActionResult<Project>> GetById(Guid id)
    {
        var project = await projectRepository.GetProject(id);

        if (project is null)
            return NotFound();

        return Ok(project);
    }
    
    [HttpGet("activeProjects")]
    public async Task<IActionResult> GetActiveProjects()
    {
        var activeProjects = await projectRepository.GetActiveProjects();
        
        if (activeProjects.Count == 0) return NotFound();
        
        return Ok(activeProjects);
    }

    [HttpGet("activeProjectsCount")]
    public async Task<IActionResult> GetActiveProjectsCount()
    {
        var activeProjectsCount = await projectRepository.GetActiveProjectsCount();
        
        if (activeProjectsCount == 0) return NotFound();
        
        return Ok(activeProjectsCount);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectDto dto)
    {
        var project = await projectRepository.CreateProjectAsync(dto);
        
        return CreatedAtRoute(nameof(GetById), new { id = project.Id }, project);
    }
    
}