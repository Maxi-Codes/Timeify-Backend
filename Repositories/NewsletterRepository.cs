using Microsoft.EntityFrameworkCore;
using Npgsql;
using timeify_rest.Entities;
using timeify_rest.Infrastructure.Data;
using timeify_rest.Interfaces;

namespace timeify_rest.Repositories;

public class NewsletterRepository(AppDbContext appDbContext) : INewsletterRepository
{
    public async Task<bool> SubscribeAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var subscriber = await appDbContext.NewsletterSubscribers
            .FirstOrDefaultAsync(s => s.Email == normalizedEmail);

        if (subscriber is not null)
        {
            if (subscriber.IsActive)
                return false;

            subscriber.IsActive = true;
            subscriber.SubscribedAt = DateTime.UtcNow;
            subscriber.UnsubscribedAt = null;

            await appDbContext.SaveChangesAsync();
            return true;
        }

        subscriber = new NewsletterSubscriber
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            SubscribedAt = DateTime.UtcNow,
            IsActive = true
        };

        appDbContext.NewsletterSubscribers.Add(subscriber);

        try
        {
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
                  {
                      SqlState: PostgresErrorCodes.UniqueViolation
                  })
        {
            // A concurrent request subscribed the same normalized email first.
            appDbContext.Entry(subscriber).State = EntityState.Detached;
            return false;
        }
    }
}
