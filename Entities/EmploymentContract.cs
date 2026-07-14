namespace timeify_rest.Entities;

public class EmploymentContract
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal WeeklyHours { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}