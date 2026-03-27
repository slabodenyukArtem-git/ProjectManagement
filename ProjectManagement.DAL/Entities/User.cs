using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.DAL.Entities;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    
    // Связь с сотрудником (если пользователь является сотрудником)
    public int? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
}
