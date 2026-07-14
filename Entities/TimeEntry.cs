namespace timeify_rest.Entities;

public class TimeEntry
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateOnly Date { get; set; }

    public int MinutesWorked { get; set; }
    public int BreakMinutes { get; set; }

    public string? Comment { get; set; }
}