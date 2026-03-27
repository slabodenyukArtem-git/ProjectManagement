using System;
using System.Collections.Generic;

namespace ProjectManagement.BLL.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int? ExecutorId { get; set; }
    public string? ExecutorName { get; set; }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public int Priority { get; set; } = 5;
    public string Status { get; set; } = "ToDo";
    public int ProjectId { get; set; }
    public int AuthorId { get; set; }
    public int? ExecutorId { get; set; }
}

public class UpdateTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ExecutorId { get; set; }
}

public class TaskFilterDto
{
    public string? Status { get; set; }
    public int? Priority { get; set; }
    public int? ProjectId { get; set; }
    public int? ExecutorId { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
