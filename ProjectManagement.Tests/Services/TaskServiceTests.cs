using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL;
using ProjectManagement.DAL.Entities;
using Xunit;
using SystemTask = System.Threading.Tasks.Task;
using TaskEntity = ProjectManagement.DAL.Entities.Task;
using TaskStatusEnum = ProjectManagement.DAL.Entities.TaskStatus;

namespace ProjectManagement.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _service = new TaskService(_context);
        
        _context.Employees.AddRange(
            new Employee { Id = 1, FirstName = "Иван", LastName = "Петров", Email = "ivan@test.com" },
            new Employee { Id = 2, FirstName = "Мария", LastName = "Сидорова", Email = "maria@test.com" },
            new Employee { Id = 3, FirstName = "Алексей", LastName = "Смирнов", Email = "alexey@test.com" }
        );
        
        _context.Projects.Add(new Project
        {
            Id = 1,
            Name = "Тестовый проект",
            CustomerCompany = "ООО Тест",
            ExecutorCompany = "ООО Исполнитель",
            StartDate = System.DateTime.UtcNow,
            EndDate = System.DateTime.UtcNow.AddMonths(6),
            Priority = 5,
            ProjectManagerId = 1
        });
        _context.SaveChanges();
    }

    [Fact]
    public async SystemTask GetAllAsync_ShouldReturnAllTasks()
    {
        _context.Tasks.AddRange(
            new TaskEntity
            {
                Title = "Задача 1",
                Priority = 5,
                Status = TaskStatusEnum.ToDo,
                ProjectId = 1,
                AuthorId = 1,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            },
            new TaskEntity
            {
                Title = "Задача 2",
                Priority = 8,
                Status = TaskStatusEnum.InProgress,
                ProjectId = 1,
                AuthorId = 1,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            });
        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async SystemTask GetByIdAsync_WithValidId_ShouldReturnTask()
    {
        var task = new TaskEntity
        {
            Title = "Тестовая задача",
            Priority = 5,
            Status = TaskStatusEnum.ToDo,
            ProjectId = 1,
            AuthorId = 1,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(task.Id);
        Assert.NotNull(result);
        Assert.Equal(task.Title, result.Title);
    }

    [Fact]
    public async SystemTask GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async SystemTask CreateAsync_ShouldAddNewTask()
    {
        var dto = new CreateTaskDto
        {
            Title = "Новая задача",
            Comment = "Тестовый комментарий",
            Priority = 8,
            Status = "InProgress",
            ProjectId = 1,
            AuthorId = 1,
            ExecutorId = 2
        };

        var result = await _service.CreateAsync(dto);
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Priority, result.Priority);
    }

    [Fact]
    public async SystemTask UpdateAsync_WithValidId_ShouldUpdateTask()
    {
        var task = new TaskEntity
        {
            Title = "Старое название",
            Priority = 3,
            Status = TaskStatusEnum.ToDo,
            ProjectId = 1,
            AuthorId = 1,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateTaskDto
        {
            Id = task.Id,
            Title = "Новое название",
            Comment = "Обновленный комментарий",
            Priority = 10,
            Status = "Done",
            ExecutorId = 2
        };

        var result = await _service.UpdateAsync(updateDto);
        Assert.NotNull(result);
        Assert.Equal(updateDto.Title, result.Title);
        Assert.Equal(updateDto.Priority, result.Priority);
    }

    [Fact]
    public async SystemTask DeleteAsync_WithValidId_ShouldRemoveTask()
    {
        var task = new TaskEntity
        {
            Title = "Удаляемая задача",
            Priority = 5,
            Status = TaskStatusEnum.ToDo,
            ProjectId = 1,
            AuthorId = 1,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(task.Id);
        Assert.True(result);
        var deleted = await _context.Tasks.FindAsync(task.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async SystemTask ChangeStatusAsync_ShouldUpdateTaskStatus()
    {
        var task = new TaskEntity
        {
            Title = "Задача для смены статуса",
            Priority = 5,
            Status = TaskStatusEnum.ToDo,
            ProjectId = 1,
            AuthorId = 1,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = await _service.ChangeStatusAsync(task.Id, "InProgress");
        Assert.True(result);
        var updated = await _context.Tasks.FindAsync(task.Id);
        Assert.Equal(TaskStatusEnum.InProgress, updated.Status);
    }

    [Fact]
    public async SystemTask GetTasksByProjectAsync_ShouldReturnTasksForProject()
    {
        _context.Tasks.AddRange(
            new TaskEntity
            {
                Title = "Задача проекта 1",
                Priority = 5,
                Status = TaskStatusEnum.ToDo,
                ProjectId = 1,
                AuthorId = 1,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            },
            new TaskEntity
            {
                Title = "Другая задача",
                Priority = 3,
                Status = TaskStatusEnum.InProgress,
                ProjectId = 1,
                AuthorId = 1,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            });
        await _context.SaveChangesAsync();

        var result = await _service.GetTasksByProjectAsync(1);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async SystemTask GetAllAsync_WithFilter_ShouldReturnFilteredTasks()
    {
        _context.Tasks.AddRange(
            new TaskEntity
            {
                Title = "Задача с высоким приоритетом",
                Priority = 10,
                Status = TaskStatusEnum.InProgress,
                ProjectId = 1,
                AuthorId = 1,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            },
            new TaskEntity
            {
                Title = "Задача с низким приоритетом",
                Priority = 2,
                Status = TaskStatusEnum.ToDo,
                ProjectId = 1,
                AuthorId = 1,
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow
            });
        await _context.SaveChangesAsync();

        var filter = new TaskFilterDto
        {
            Priority = 10,
            Status = "InProgress"
        };

        var result = await _service.GetAllAsync(filter);
        Assert.Single(result);
        Assert.Equal(10, result.First().Priority);
        Assert.Equal("InProgress", result.First().Status);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
