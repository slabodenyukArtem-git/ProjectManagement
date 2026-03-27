using System;

namespace ProjectManagement.DAL.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public int Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int ProjectId { get; set; }
    public int AuthorId { get; set; }
    public int? ExecutorId { get; set; }
    
    public virtual Project Project { get; set; } = null!;
    public virtual Employee Author { get; set; } = null!;
    public virtual Employee? Executor { get; set; }
}

public enum TaskStatus
{
    ToDo = 0,
    InProgress = 1,
    Done = 2
}
