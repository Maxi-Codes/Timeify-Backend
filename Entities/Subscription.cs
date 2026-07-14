
using timeify_rest.Enums;

namespace timeify_rest.Entities;

public class Subscription
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public SubscriptionPlan Plan { get; set; }
    public SubscriptionStatus Status { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? EndsAt { get; set; }
}