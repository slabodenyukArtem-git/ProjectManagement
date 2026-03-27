using System;
using System.Collections.Generic;

namespace ProjectManagement.BLL.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CustomerCompany { get; set; } = string.Empty;
    public string ExecutorCompany { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public int ProjectManagerId { get; set; }
    public string ProjectManagerName { get; set; } = string.Empty;
    public List<EmployeeDto> Executors { get; set; } = new List<EmployeeDto>();
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string CustomerCompany { get; set; } = string.Empty;
    public string ExecutorCompany { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public int ProjectManagerId { get; set; }
    public List<int> ExecutorIds { get; set; } = new List<int>();
}

public class UpdateProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CustomerCompany { get; set; } = string.Empty;
    public string ExecutorCompany { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public int ProjectManagerId { get; set; }
    public List<int> ExecutorIds { get; set; } = new List<int>();
}

public class ProjectFilterDto
{
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public int? Priority { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
