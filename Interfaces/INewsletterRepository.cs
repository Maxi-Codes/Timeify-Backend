namespace timeify_rest.Interfaces;

public interface INewsletterRepository
{
    Task<bool> SubscribeAsync(string email);
}
