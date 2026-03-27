using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Interfaces;
using ProjectManagement.DAL;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.BLL.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(ProjectFilterDto? filter = null)
    {
        var query = _context.Projects
            .Include(p => p.ProjectManager)
            .Include(p => p.Executors)
            .AsQueryable();

        if (filter != null)
        {
            if (filter.StartDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= filter.StartDateFrom.Value);
            
            if (filter.StartDateTo.HasValue)
                query = query.Where(p => p.StartDate <= filter.StartDateTo.Value);
            
            if (filter.Priority.HasValue)
                query = query.Where(p => p.Priority == filter.Priority.Value);
            
            // Apply sorting only if SortBy is provided
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "name" => filter.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "startdate" => filter.SortDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                    "priority" => filter.SortDescending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }
        }
        else
        {
            query = query.OrderBy(p => p.Id);
        }

        var projects = await query.ToListAsync();
        
        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            CustomerCompany = p.CustomerCompany,
            ExecutorCompany = p.ExecutorCompany,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Priority = p.Priority,
            ProjectManagerId = p.ProjectManagerId,
            ProjectManagerName = p.ProjectManager != null ? $"{p.ProjectManager.LastName} {p.ProjectManager.FirstName}" : "",
            Executors = p.Executors.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Patronymic = e.Patronymic,
                Email = e.Email,
                FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}".Trim()
            }).ToList()
        });
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectManager)
            .Include(p => p.Executors)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return null;

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            CustomerCompany = project.CustomerCompany,
            ExecutorCompany = project.ExecutorCompany,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Priority = project.Priority,
            ProjectManagerId = project.ProjectManagerId,
            ProjectManagerName = project.ProjectManager != null ? $"{project.ProjectManager.LastName} {project.ProjectManager.FirstName}" : "",
            Executors = project.Executors.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Patronymic = e.Patronymic,
                Email = e.Email,
                FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}".Trim()
            }).ToList()
        };
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            CustomerCompany = dto.CustomerCompany,
            ExecutorCompany = dto.ExecutorCompany,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Priority = dto.Priority,
            ProjectManagerId = dto.ProjectManagerId
        };

        if (dto.ExecutorIds != null && dto.ExecutorIds.Any())
        {
            var executors = await _context.Employees
                .Where(e => dto.ExecutorIds.Contains(e.Id))
                .ToListAsync();
            
            foreach (var executor in executors)
            {
                project.Executors.Add(executor);
            }
        }

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(project.Id);
    }

    public async Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto)
    {
        var project = await _context.Projects
            .Include(p => p.Executors)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (project == null) return null;

        project.Name = dto.Name;
        project.CustomerCompany = dto.CustomerCompany;
        project.ExecutorCompany = dto.ExecutorCompany;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.Priority = dto.Priority;
        project.ProjectManagerId = dto.ProjectManagerId;

        if (dto.ExecutorIds != null)
        {
            project.Executors.Clear();
            var executors = await _context.Employees
                .Where(e => dto.ExecutorIds.Contains(e.Id))
                .ToListAsync();
            
            foreach (var executor in executors)
            {
                project.Executors.Add(executor);
            }
        }

        await _context.SaveChangesAsync();
        return await GetByIdAsync(project.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddExecutorAsync(int projectId, int employeeId)
    {
        var project = await _context.Projects
            .Include(p => p.Executors)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        
        var employee = await _context.Employees.FindAsync(employeeId);
        
        if (project == null || employee == null) return false;
        
        if (!project.Executors.Contains(employee))
        {
            project.Executors.Add(employee);
            await _context.SaveChangesAsync();
        }
        
        return true;
    }

    public async Task<bool> RemoveExecutorAsync(int projectId, int employeeId)
    {
        var project = await _context.Projects
            .Include(p => p.Executors)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        
        var employee = await _context.Employees.FindAsync(employeeId);
        
        if (project == null || employee == null) return false;
        
        if (project.Executors.Contains(employee))
        {
            project.Executors.Remove(employee);
            await _context.SaveChangesAsync();
        }
        
        return true;
    }
}
