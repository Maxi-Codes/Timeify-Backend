using timeify_rest.DTOs;
using timeify_rest.Entities;
using timeify_rest.Enums;

namespace timeify_rest.Interfaces;

public interface IAbsenceRepository
{
    Task<List<Absence>> GetAbsencesAsync(
        Guid? userId = null,
        AbsenceStatus? status = null,
        DateOnly? from = null,
        DateOnly? to = null);

    Task<Absence?> GetAbsenceAsync(Guid id);
    Task<Absence> CreateAbsenceAsync(CreateAbsenceDto dto);
    Task<Absence?> UpdateAbsenceAsync(Guid id, UpdateAbsenceDto dto);
    Task<Absence?> UpdateAbsenceStatusAsync(Guid id, AbsenceStatus status);
    Task<bool> DeleteAbsenceAsync(Guid id);
}
