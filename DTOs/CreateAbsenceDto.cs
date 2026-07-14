using System.ComponentModel.DataAnnotations;
using timeify_rest.Enums;

namespace timeify_rest.DTOs;

public class CreateAbsenceDto
{
    public Guid UserId { get; set; }
    public AbsenceType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    [Url]
    public string? AttachmentUrl { get; set; }
}
