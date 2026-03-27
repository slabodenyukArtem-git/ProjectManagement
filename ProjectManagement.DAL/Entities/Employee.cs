namespace ProjectManagement.DAL.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public string Email { get; set; } = string.Empty;
    
    public virtual ICollection<Project> LeadingProjects { get; set; } = new List<Project>();
    public virtual ICollection<Project> ParticipatingProjects { get; set; } = new List<Project>();
}
