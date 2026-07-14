using System.ComponentModel.DataAnnotations;

namespace timeify_rest.DTOs;

public class UpdateTimeEntryDto
{
    public Guid ProjectId { get; set; }
    public DateOnly Date { get; set; }

    [Range(1, 1440)]
    public int MinutesWorked { get; set; }

    [Range(0, 1440)]
    public int BreakMinutes { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}
