using Microsoft.EntityFrameworkCore;
using timeify_rest.Entities;

namespace timeify_rest.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    
    // DbSets
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<User> Users => Set<User>();
    public DbSet<EmploymentContract> EmploymentContracts => Set<EmploymentContract>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ================= COMPANY =================
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Projects)
            .WithOne(p => p.Company)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Company>()
            .HasOne<Subscription>()
            .WithOne(s => s.Company)
            .HasForeignKey<Subscription>(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ================= USER =================
        modelBuilder.Entity<User>()
            .HasMany<TimeEntry>()
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany<Absence>()
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.EmploymentContract)
            .WithOne(c => c.User)
            .HasForeignKey<EmploymentContract>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ================= PROJECT =================
        modelBuilder.Entity<Project>()
            .HasMany<TimeEntry>()
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Address Value Object
        modelBuilder.Entity<Project>()
            .OwnsOne(p => p.Address);

        // ================= TIME ENTRY =================
        modelBuilder.Entity<TimeEntry>()
            .HasIndex(t => new { t.UserId, t.Date });

        modelBuilder.Entity<TimeEntry>()
            .HasIndex(t => t.ProjectId);

        modelBuilder.Entity<TimeEntry>()
            .Property(t => t.BreakMinutes)
            .HasDefaultValue(0);

        // ================= ABSENCE =================
        modelBuilder.Entity<Absence>()
            .HasIndex(a => new { a.UserId, a.StartDate, a.EndDate });

        // ================= NEWSLETTER =================
        modelBuilder.Entity<NewsletterSubscriber>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<NewsletterSubscriber>()
            .Property(s => s.Email)
            .HasMaxLength(320)
            .IsRequired();

        // ================= ENUM CONVERSIONS =================
        modelBuilder.Entity<Absence>()
            .Property(a => a.Type)
            .HasConversion<int>();

        modelBuilder.Entity<Absence>()
            .Property(a => a.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Subscription>()
            .Property(s => s.Plan)
            .HasConversion<int>();

        modelBuilder.Entity<Subscription>()
            .Property(s => s.Status)
            .HasConversion<int>();
    }
}
