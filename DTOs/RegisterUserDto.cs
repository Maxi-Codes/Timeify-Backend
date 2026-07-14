namespace timeify_rest.DTOs;

public class RegisterUserDto
{
    public Guid CompanyId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Role { get; set; }
}