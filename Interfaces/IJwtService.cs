using timeify_rest.Entities;

namespace timeify_rest.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}