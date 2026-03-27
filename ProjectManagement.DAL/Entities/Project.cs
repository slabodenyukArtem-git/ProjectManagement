using Task = ProjectManagement.DAL.Entities.Task;

namespace ProjectManagement.DAL.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CustomerCompany { get; set; } = string.Empty;
    public string ExecutorCompany { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }

    public int ProjectManagerId { get; set; }
    public virtual Employee ProjectManager { get; set; } = null!;

    public virtual ICollection<Employee> Executors { get; set; } = new List<Employee>();
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
