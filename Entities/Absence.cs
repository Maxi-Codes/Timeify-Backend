using timeify_rest.Enums;

namespace timeify_rest.Entities;

public class Absence
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public AbsenceType Type { get; set; }
    public AbsenceStatus Status { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public string? AttachmentUrl { get; set; }
}