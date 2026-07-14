using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/time-entries")]
public class TimeEntryController(ITimeEntryRepository timeEntryRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TimeEntry>>> GetTimeEntries(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? projectId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var validationResult = ValidateFilters(userId, projectId, from, to);
        if (validationResult is not null)
            return validationResult;

        var timeEntries = await timeEntryRepository.GetTimeEntriesAsync(userId, projectId, from, to);
        return Ok(timeEntries);
    }

    [HttpGet("{id:guid}", Name = nameof(GetTimeEntryById))]
    public async Task<ActionResult<TimeEntry>> GetTimeEntryById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Time entry ID must not be empty.");

        var timeEntry = await timeEntryRepository.GetTimeEntryAsync(id);

        if (timeEntry is null)
            return NotFound($"Time entry with ID '{id}' was not found.");

        return Ok(timeEntry);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<TimeEntry>>> GetByUserId(
        Guid userId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var validationResult = ValidateFilters(userId, null, from, to);
        if (validationResult is not null)
            return validationResult;

        var timeEntries = await timeEntryRepository.GetTimeEntriesAsync(
            userId: userId,
            from: from,
            to: to);

        return Ok(timeEntries);
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<List<TimeEntry>>> GetByProjectId(
        Guid projectId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var validationResult = ValidateFilters(null, projectId, from, to);
        if (validationResult is not null)
            return validationResult;

        var timeEntries = await timeEntryRepository.GetTimeEntriesAsync(
            projectId: projectId,
            from: from,
            to: to);

        return Ok(timeEntries);
    }

    [HttpPost]
    public async Task<ActionResult<TimeEntry>> CreateTimeEntry(CreateTimeEntryDto dto)
    {
        var validationResult = ValidateTimeEntry(
            dto.UserId,
            dto.ProjectId,
            dto.Date,
            dto.MinutesWorked,
            dto.BreakMinutes,
            dto.Comment);

        if (validationResult is not null)
            return validationResult;

        try
        {
            var timeEntry = await timeEntryRepository.CreateTimeEntryAsync(dto);
            return CreatedAtRoute(nameof(GetTimeEntryById), new { id = timeEntry.Id }, timeEntry);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TimeEntry>> UpdateTimeEntry(Guid id, UpdateTimeEntryDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("Time entry ID must not be empty.");

        var validationResult = ValidateTimeEntry(
            null,
            dto.ProjectId,
            dto.Date,
            dto.MinutesWorked,
            dto.BreakMinutes,
            dto.Comment);

        if (validationResult is not null)
            return validationResult;

        try
        {
            var timeEntry = await timeEntryRepository.UpdateTimeEntryAsync(id, dto);

            if (timeEntry is null)
                return NotFound($"Time entry with ID '{id}' was not found.");

            return Ok(timeEntry);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTimeEntry(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Time entry ID must not be empty.");

        var deleted = await timeEntryRepository.DeleteTimeEntryAsync(id);

        if (!deleted)
            return NotFound($"Time entry with ID '{id}' was not found.");

        return NoContent();
    }

    private BadRequestObjectResult? ValidateFilters(
        Guid? userId,
        Guid? projectId,
        DateOnly? from,
        DateOnly? to)
    {
        if (userId == Guid.Empty)
            return BadRequest("UserId must not be empty.");

        if (projectId == Guid.Empty)
            return BadRequest("ProjectId must not be empty.");

        if (from.HasValue && to.HasValue && from.Value > to.Value)
            return BadRequest("'from' must be before or equal to 'to'.");

        return null;
    }

    private BadRequestObjectResult? ValidateTimeEntry(
        Guid? userId,
        Guid projectId,
        DateOnly date,
        int minutesWorked,
        int breakMinutes,
        string? comment)
    {
        if (userId.HasValue && userId.Value == Guid.Empty)
            return BadRequest("UserId must not be empty.");

        if (projectId == Guid.Empty)
            return BadRequest("ProjectId must not be empty.");

        if (date == default)
            return BadRequest("Date is required.");

        if (minutesWorked <= 0 || minutesWorked > 1440)
            return BadRequest("MinutesWorked must be between 1 and 1440.");

        if (breakMinutes < 0 || breakMinutes > 1440)
            return BadRequest("BreakMinutes must be between 0 and 1440.");

        if (minutesWorked + breakMinutes > 1440)
            return BadRequest("MinutesWorked and BreakMinutes must not exceed 1440 minutes in total.");

        if (comment?.Length > 1000)
            return BadRequest("Comment must not exceed 1000 characters.");

        return null;
    }
}
