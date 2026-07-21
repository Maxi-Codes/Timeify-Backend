namespace timeify_rest.Entities;

public class NewsletterSubscriber
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime SubscribedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UnsubscribedAt { get; set; }
}
