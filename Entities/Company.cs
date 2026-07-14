using timeify_rest.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? Street { get; set; }
    public int? HouseNumber { get; set; }
    public int? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}