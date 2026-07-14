using Microsoft.EntityFrameworkCore;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Enums;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class AbsenceRepository(AppDbContext appDbContext) : IAbsenceRepository
{
    public async Task<List<Absence>> GetAbsencesAsync(
        Guid? userId = null,
        AbsenceStatus? status = null,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        var query = appDbContext.Absences
            .AsNoTracking()
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        // Return every absence which overlaps the requested date range.
        if (from.HasValue)
            query = query.Where(a => a.EndDate >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.StartDate <= to.Value);

        return await query
            .OrderByDescending(a => a.StartDate)
            .ThenBy(a => a.EndDate)
            .ToListAsync();
    }

    public async Task<Absence?> GetAbsenceAsync(Guid id)
    {
        return await appDbContext.Absences
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Absence> CreateAbsenceAsync(CreateAbsenceDto dto)
    {
        var userExists = await appDbContext.Users.AnyAsync(u => u.Id == dto.UserId);

        if (!userExists)
            throw new KeyNotFoundException($"User with ID '{dto.UserId}' was not found.");

        var absence = new Absence
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Type = dto.Type,
            Status = AbsenceStatus.Requested,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            AttachmentUrl = dto.AttachmentUrl
        };

        appDbContext.Absences.Add(absence);
        await appDbContext.SaveChangesAsync();

        return absence;
    }

    public async Task<Absence?> UpdateAbsenceAsync(Guid id, UpdateAbsenceDto dto)
    {
        var absence = await appDbContext.Absences.FindAsync(id);

        if (absence is null)
            return null;

        absence.Type = dto.Type;
        absence.StartDate = dto.StartDate;
        absence.EndDate = dto.EndDate;
        absence.AttachmentUrl = dto.AttachmentUrl;

        await appDbContext.SaveChangesAsync();
        return absence;
    }

    public async Task<Absence?> UpdateAbsenceStatusAsync(Guid id, AbsenceStatus status)
    {
        var absence = await appDbContext.Absences.FindAsync(id);

        if (absence is null)
            return null;

        absence.Status = status;

        await appDbContext.SaveChangesAsync();
        return absence;
    }

    public async Task<bool> DeleteAbsenceAsync(Guid id)
    {
        var absence = await appDbContext.Absences.FindAsync(id);

        if (absence is null)
            return false;

        appDbContext.Absences.Remove(absence);
        await appDbContext.SaveChangesAsync();
        return true;
    }
}
