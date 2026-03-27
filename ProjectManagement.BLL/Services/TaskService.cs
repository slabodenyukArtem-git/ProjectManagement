using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Interfaces;
using ProjectManagement.DAL;
using ProjectManagement.DAL.Entities;
using TaskEntity = ProjectManagement.DAL.Entities.Task;
using TaskStatusEnum = ProjectManagement.DAL.Entities.TaskStatus;

namespace ProjectManagement.BLL.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync(TaskFilterDto? filter = null)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Author)
            .Include(t => t.Executor)
            .AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Status))
            {
                var status = (TaskStatusEnum)Enum.Parse(typeof(TaskStatusEnum), filter.Status);
                query = query.Where(t => t.Status == status);
            }
            
            if (filter.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.Priority.Value);
            }
            
            if (filter.ProjectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == filter.ProjectId.Value);
            }
            
            if (filter.ExecutorId.HasValue)
            {
                query = query.Where(t => t.ExecutorId == filter.ExecutorId.Value);
            }
            
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "title" => filter.SortDescending 
                        ? query.OrderByDescending(t => t.Title) 
                        : query.OrderBy(t => t.Title),
                    "priority" => filter.SortDescending 
                        ? query.OrderByDescending(t => t.Priority) 
                        : query.OrderBy(t => t.Priority),
                    "status" => filter.SortDescending 
                        ? query.OrderByDescending(t => t.Status) 
                        : query.OrderBy(t => t.Status),
                    "createdat" => filter.SortDescending 
                        ? query.OrderByDescending(t => t.CreatedAt) 
                        : query.OrderBy(t => t.CreatedAt),
                    _ => query.OrderBy(t => t.Id)
                };
            }
        }

        var tasks = await query.ToListAsync();
        
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto?> GetByIdAsync(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Author)
            .Include(t => t.Executor)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task != null ? MapToDto(task) : null;
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
    {
        var task = new TaskEntity
        {
            Title = dto.Title,
            Comment = dto.Comment,
            Priority = dto.Priority,
            Status = (TaskStatusEnum)Enum.Parse(typeof(TaskStatusEnum), dto.Status),
            ProjectId = dto.ProjectId,
            AuthorId = dto.AuthorId,
            ExecutorId = dto.ExecutorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(task.Id) ?? throw new Exception("Failed to create task");
    }

    public async Task<TaskDto?> UpdateAsync(UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(dto.Id);
        if (task == null) return null;

        task.Title = dto.Title;
        task.Comment = dto.Comment;
        task.Priority = dto.Priority;
        task.Status = (TaskStatusEnum)Enum.Parse(typeof(TaskStatusEnum), dto.Status);
        task.ExecutorId = dto.ExecutorId;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(task.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int taskId, string newStatus)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        task.Status = (TaskStatusEnum)Enum.Parse(typeof(TaskStatusEnum), newStatus);
        task.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(int projectId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Author)
            .Include(t => t.Executor)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByExecutorAsync(int executorId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Author)
            .Include(t => t.Executor)
            .Where(t => t.ExecutorId == executorId)
            .ToListAsync();

        return tasks.Select(MapToDto);
    }

    private static TaskDto MapToDto(TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Comment = task.Comment,
            Priority = task.Priority,
            Status = task.Status.ToString(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name ?? string.Empty,
            AuthorId = task.AuthorId,
            AuthorName = task.Author != null 
                ? $"{task.Author.LastName} {task.Author.FirstName}" 
                : string.Empty,
            ExecutorId = task.ExecutorId,
            ExecutorName = task.Executor != null 
                ? $"{task.Executor.LastName} {task.Executor.FirstName}" 
                : null
        };
    }
}
