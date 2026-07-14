using Microsoft.EntityFrameworkCore;
using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class TimeEntryRepository(AppDbContext appDbContext) : ITimeEntryRepository
{
    public async Task<List<TimeEntry>> GetTimeEntriesAsync(
        Guid? userId = null,
        Guid? projectId = null,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        var query = appDbContext.TimeEntries
            .AsNoTracking()
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        if (from.HasValue)
            query = query.Where(t => t.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.Date <= to.Value);

        return await query
            .OrderByDescending(t => t.Date)
            .ThenBy(t => t.UserId)
            .ToListAsync();
    }

    public async Task<TimeEntry?> GetTimeEntryAsync(Guid id)
    {
        return await appDbContext.TimeEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TimeEntry> CreateTimeEntryAsync(CreateTimeEntryDto dto)
    {
        var user = await appDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user is null)
            throw new KeyNotFoundException($"User with ID '{dto.UserId}' was not found.");

        var project = await appDbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

        if (project is null)
            throw new KeyNotFoundException($"Project with ID '{dto.ProjectId}' was not found.");

        if (!project.IsActive)
            throw new InvalidOperationException("Time entries cannot be created for an inactive project.");

        if (user.CompanyId != project.CompanyId)
            throw new InvalidOperationException("User and project must belong to the same company.");

        await EnsureDailyLimitAsync(
            dto.UserId,
            dto.Date,
            dto.MinutesWorked,
            dto.BreakMinutes);

        var timeEntry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            ProjectId = dto.ProjectId,
            Date = dto.Date,
            MinutesWorked = dto.MinutesWorked,
            BreakMinutes = dto.BreakMinutes,
            Comment = NormalizeComment(dto.Comment)
        };

        appDbContext.TimeEntries.Add(timeEntry);
        await appDbContext.SaveChangesAsync();

        return timeEntry;
    }

    public async Task<TimeEntry?> UpdateTimeEntryAsync(Guid id, UpdateTimeEntryDto dto)
    {
        var timeEntry = await appDbContext.TimeEntries.FindAsync(id);

        if (timeEntry is null)
            return null;

        var project = await appDbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

        if (project is null)
            throw new KeyNotFoundException($"Project with ID '{dto.ProjectId}' was not found.");

        var userCompanyId = await appDbContext.Users
            .Where(u => u.Id == timeEntry.UserId)
            .Select(u => (Guid?)u.CompanyId)
            .FirstOrDefaultAsync();

        if (!userCompanyId.HasValue)
            throw new KeyNotFoundException($"User with ID '{timeEntry.UserId}' was not found.");

        if (userCompanyId.Value != project.CompanyId)
            throw new InvalidOperationException("User and project must belong to the same company.");

        if (timeEntry.ProjectId != project.Id && !project.IsActive)
            throw new InvalidOperationException("A time entry cannot be moved to an inactive project.");

        await EnsureDailyLimitAsync(
            timeEntry.UserId,
            dto.Date,
            dto.MinutesWorked,
            dto.BreakMinutes,
            excludeId: timeEntry.Id);

        timeEntry.ProjectId = dto.ProjectId;
        timeEntry.Date = dto.Date;
        timeEntry.MinutesWorked = dto.MinutesWorked;
        timeEntry.BreakMinutes = dto.BreakMinutes;
        timeEntry.Comment = NormalizeComment(dto.Comment);

        await appDbContext.SaveChangesAsync();
        return timeEntry;
    }

    public async Task<bool> DeleteTimeEntryAsync(Guid id)
    {
        var timeEntry = await appDbContext.TimeEntries.FindAsync(id);

        if (timeEntry is null)
            return false;

        appDbContext.TimeEntries.Remove(timeEntry);
        await appDbContext.SaveChangesAsync();
        return true;
    }

    private static string? NormalizeComment(string? comment)
    {
        return string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }

    private async Task EnsureDailyLimitAsync(
        Guid userId,
        DateOnly date,
        int minutesWorked,
        int breakMinutes,
        Guid? excludeId = null)
    {
        var query = appDbContext.TimeEntries
            .Where(t => t.UserId == userId && t.Date == date);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        var existingMinutes = await query
            .SumAsync(t => (long)t.MinutesWorked + t.BreakMinutes);

        if (existingMinutes + minutesWorked + breakMinutes > 1440)
        {
            throw new InvalidOperationException(
                "The user's time entries must not exceed 1440 minutes per day including breaks.");
        }
    }
}
