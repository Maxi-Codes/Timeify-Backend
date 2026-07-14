using Microsoft.AspNetCore.Mvc;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Enums;
using timeify_rest.Interfaces;

namespace timeify_rest.Controller;

[ApiController]
[Route("api/absences")]
public class AbsenceController(IAbsenceRepository absenceRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Absence>>> GetAbsences(
        [FromQuery] Guid? userId,
        [FromQuery] AbsenceStatus? status,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        if (status.HasValue && !Enum.IsDefined(status.Value))
            return BadRequest("The absence status is invalid.");

        if (from.HasValue && to.HasValue && from.Value > to.Value)
            return BadRequest("'from' must be before or equal to 'to'.");

        var absences = await absenceRepository.GetAbsencesAsync(userId, status, from, to);
        return Ok(absences);
    }

    [HttpGet("{id:guid}", Name = nameof(GetAbsenceById))]
    public async Task<ActionResult<Absence>> GetAbsenceById(Guid id)
    {
        var absence = await absenceRepository.GetAbsenceAsync(id);

        if (absence is null)
            return NotFound();

        return Ok(absence);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<Absence>>> GetByUserId(Guid userId)
    {
        var absences = await absenceRepository.GetAbsencesAsync(userId: userId);
        return Ok(absences);
    }

    [HttpPost]
    public async Task<ActionResult<Absence>> CreateAbsence(CreateAbsenceDto dto)
    {
        var validationResult = ValidateAbsence(dto.UserId, dto.Type, dto.StartDate, dto.EndDate);
        if (validationResult is not null)
            return validationResult;

        try
        {
            var absence = await absenceRepository.CreateAbsenceAsync(dto);
            return CreatedAtRoute(nameof(GetAbsenceById), new { id = absence.Id }, absence);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Absence>> UpdateAbsence(Guid id, UpdateAbsenceDto dto)
    {
        var validationResult = ValidateAbsenceTypeAndDates(dto.Type, dto.StartDate, dto.EndDate);
        if (validationResult is not null)
            return validationResult;

        var absence = await absenceRepository.UpdateAbsenceAsync(id, dto);

        if (absence is null)
            return NotFound();

        return Ok(absence);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<Absence>> UpdateStatus(Guid id, UpdateAbsenceStatusDto dto)
    {
        if (!Enum.IsDefined(dto.Status))
            return BadRequest("The absence status is invalid.");

        var absence = await absenceRepository.UpdateAbsenceStatusAsync(id, dto.Status);

        if (absence is null)
            return NotFound();

        return Ok(absence);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAbsence(Guid id)
    {
        var deleted = await absenceRepository.DeleteAbsenceAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private BadRequestObjectResult? ValidateAbsence(
        Guid userId,
        AbsenceType type,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (userId == Guid.Empty)
            return BadRequest("UserId must not be empty.");

        return ValidateAbsenceTypeAndDates(type, startDate, endDate);
    }

    private BadRequestObjectResult? ValidateAbsenceTypeAndDates(
        AbsenceType type,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (!Enum.IsDefined(type))
            return BadRequest("The absence type is invalid.");

        if (startDate == default || endDate == default)
            return BadRequest("StartDate and EndDate are required.");

        if (startDate > endDate)
            return BadRequest("StartDate must be before or equal to EndDate.");

        return null;
    }
}
