using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectRepository projectRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetProjects(
        [FromQuery] Guid? companyId,
        [FromQuery] bool? isActive)
    {
        if (companyId == Guid.Empty)
            return BadRequest("CompanyId must not be empty.");

        var projects = await projectRepository.GetProjectsAsync(companyId, isActive);
        return Ok(projects);
    }

    [HttpGet("{id:guid}", Name = nameof(GetProjectById))]
    public async Task<ActionResult<Project>> GetProjectById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Project ID must not be empty.");

        var project = await projectRepository.GetProjectAsync(id);

        if (project is null)
            return NotFound($"Project with ID '{id}' was not found.");

        return Ok(project);
    }

    [HttpGet("activeProjects")]
    public async Task<ActionResult<List<Project>>> GetActiveProjects([FromQuery] Guid? companyId)
    {
        if (companyId == Guid.Empty)
            return BadRequest("CompanyId must not be empty.");

        var projects = await projectRepository.GetProjectsAsync(companyId, isActive: true);
        return Ok(projects);
    }

    [HttpGet("activeProjectsCount")]
    public async Task<ActionResult<int>> GetActiveProjectsCount([FromQuery] Guid? companyId)
    {
        if (companyId == Guid.Empty)
            return BadRequest("CompanyId must not be empty.");

        var count = await projectRepository.GetActiveProjectsCountAsync(companyId);
        return Ok(count);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(CreateProjectDto dto)
    {
        var validationResult = ValidateProject(dto.Name, dto.Address, dto.CompanyId);
        if (validationResult is not null)
            return validationResult;

        try
        {
            var project = await projectRepository.CreateProjectAsync(dto);
            return CreatedAtRoute(nameof(GetProjectById), new { id = project.Id }, project);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Project>> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("Project ID must not be empty.");

        var validationResult = ValidateProject(dto.Name, dto.Address);
        if (validationResult is not null)
            return validationResult;

        var project = await projectRepository.UpdateProjectAsync(id, dto);

        if (project is null)
            return NotFound($"Project with ID '{id}' was not found.");

        return Ok(project);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<Project>> UpdateStatus(Guid id, UpdateProjectStatusDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("Project ID must not be empty.");

        if (!dto.IsActive.HasValue)
            return BadRequest("IsActive is required.");

        var project = await projectRepository.UpdateProjectStatusAsync(id, dto.IsActive.Value);

        if (project is null)
            return NotFound($"Project with ID '{id}' was not found.");

        return Ok(project);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Project ID must not be empty.");

        try
        {
            var deleted = await projectRepository.DeleteProjectAsync(id);

            if (!deleted)
                return NotFound($"Project with ID '{id}' was not found.");

            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    private BadRequestObjectResult? ValidateProject(
        string name,
        ProjectAddressDto? address,
        Guid? companyId = null)
    {
        if (companyId.HasValue && companyId.Value == Guid.Empty)
            return BadRequest("CompanyId must not be empty.");

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Project name is required.");

        if (address is null)
            return BadRequest("Project address is required.");

        if (string.IsNullOrWhiteSpace(address.Street) ||
            string.IsNullOrWhiteSpace(address.PostalCode) ||
            string.IsNullOrWhiteSpace(address.City) ||
            string.IsNullOrWhiteSpace(address.Country))
        {
            return BadRequest("Street, PostalCode, City and Country are required.");
        }

        if (address.Latitude is < -90 or > 90)
            return BadRequest("Latitude must be between -90 and 90.");

        if (address.Longitude is < -180 or > 180)
            return BadRequest("Longitude must be between -180 and 180.");

        return null;
    }
}
