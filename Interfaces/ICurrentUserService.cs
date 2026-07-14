namespace timeify_rest.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid CompanyId { get; }
    string Role { get; }
}