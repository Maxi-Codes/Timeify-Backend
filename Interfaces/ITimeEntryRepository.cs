using timeify_rest.DTOs;
using timeify_rest.Entities;

namespace timeify_rest.Interfaces;

public interface ITimeEntryRepository
{
    Task<List<TimeEntry>> GetTimeEntriesAsync(
        Guid? userId = null,
        Guid? projectId = null,
        DateOnly? from = null,
        DateOnly? to = null);

    Task<TimeEntry?> GetTimeEntryAsync(Guid id);
    Task<TimeEntry> CreateTimeEntryAsync(CreateTimeEntryDto dto);
    Task<TimeEntry?> UpdateTimeEntryAsync(Guid id, UpdateTimeEntryDto dto);
    Task<bool> DeleteTimeEntryAsync(Guid id);
}
