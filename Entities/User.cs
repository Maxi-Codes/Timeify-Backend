using timeify_rest.Enums;

namespace timeify_rest.Entities;

public class User
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.Employee;

    public EmploymentContract? EmploymentContract { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Absence> Absences { get; set; } = new List<Absence>();
}