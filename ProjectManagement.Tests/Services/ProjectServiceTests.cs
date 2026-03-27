using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL;
using ProjectManagement.DAL.Entities;
using Xunit;
using SystemTask = System.Threading.Tasks.Task;

namespace ProjectManagement.Tests.Services;

public class ProjectServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _service = new ProjectService(_context);
        
        _context.Employees.AddRange(
            new Employee { Id = 1, FirstName = "Иван", LastName = "Петров", Email = "ivan@test.com" },
            new Employee { Id = 2, FirstName = "Мария", LastName = "Сидорова", Email = "maria@test.com" },
            new Employee { Id = 3, FirstName = "Алексей", LastName = "Смирнов", Email = "alexey@test.com" }
        );
        
        _context.Projects.AddRange(
            new Project
            {
                Id = 1,
                Name = "Тестовый проект 1",
                CustomerCompany = "ООО Ромашка",
                ExecutorCompany = "ООО Сибирские технологии",
                StartDate = new System.DateTime(2026, 4, 1),
                EndDate = new System.DateTime(2026, 12, 31),
                Priority = 5,
                ProjectManagerId = 1
            },
            new Project
            {
                Id = 2,
                Name = "Тестовый проект 2",
                CustomerCompany = "ООО ТехноСервис",
                ExecutorCompany = "ООО Сибирские технологии",
                StartDate = new System.DateTime(2026, 5, 1),
                EndDate = new System.DateTime(2026, 10, 31),
                Priority = 8,
                ProjectManagerId = 1
            }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async SystemTask GetAllAsync_ShouldReturnAllProjects()
    {
        var result = await _service.GetAllAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async SystemTask GetByIdAsync_WithValidId_ShouldReturnProject()
    {
        var result = await _service.GetByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Тестовый проект 1", result.Name);
    }

    [Fact]
    public async SystemTask GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async SystemTask CreateAsync_ShouldAddNewProject()
    {
        var dto = new CreateProjectDto
        {
            Name = "Новый проект",
            CustomerCompany = "ООО Новая",
            ExecutorCompany = "ООО Исполнитель",
            StartDate = System.DateTime.UtcNow,
            EndDate = System.DateTime.UtcNow.AddMonths(6),
            Priority = 8,
            ProjectManagerId = 1,
            ExecutorIds = new List<int> { 2, 3 }
        };
        var result = await _service.CreateAsync(dto);
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public async SystemTask UpdateAsync_WithValidId_ShouldUpdateProject()
    {
        var updateDto = new UpdateProjectDto
        {
            Id = 1,
            Name = "Обновленный проект",
            CustomerCompany = "ООО Новая компания",
            ExecutorCompany = "ООО Сибирские технологии",
            StartDate = new System.DateTime(2026, 4, 1),
            EndDate = new System.DateTime(2026, 12, 31),
            Priority = 10,
            ProjectManagerId = 1,
            ExecutorIds = new List<int>()
        };
        var result = await _service.UpdateAsync(updateDto);
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Priority, result.Priority);
    }

    [Fact]
    public async SystemTask DeleteAsync_WithValidId_ShouldRemoveProject()
    {
        var result = await _service.DeleteAsync(1);
        Assert.True(result);
        var deleted = await _context.Projects.FindAsync(1);
        Assert.Null(deleted);
    }

    [Fact]
    public async SystemTask DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _service.DeleteAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async SystemTask AddExecutorAsync_ShouldAddExecutorToProject()
    {
        var result = await _service.AddExecutorAsync(1, 2);
        Assert.True(result);
        var updatedProject = await _context.Projects
            .Include(p => p.Executors)
            .FirstOrDefaultAsync(p => p.Id == 1);
        Assert.Contains(updatedProject.Executors, e => e.Id == 2);
    }

    [Fact]
    public async SystemTask RemoveExecutorAsync_ShouldRemoveExecutorFromProject()
    {
        await _service.AddExecutorAsync(1, 2);
        var result = await _service.RemoveExecutorAsync(1, 2);
        Assert.True(result);
        var updatedProject = await _context.Projects
            .Include(p => p.Executors)
            .FirstOrDefaultAsync(p => p.Id == 1);
        Assert.DoesNotContain(updatedProject.Executors, e => e.Id == 2);
    }

    [Fact]
    public async SystemTask GetAllAsync_WithFilter_ShouldReturnFilteredProjects()
    {
        var filter = new ProjectFilterDto
        {
            StartDateFrom = new System.DateTime(2026, 4, 1),
            StartDateTo = new System.DateTime(2026, 5, 15),
            Priority = 5
        };
        var result = await _service.GetAllAsync(filter);
        Assert.Single(result);
        Assert.Equal(5, result.First().Priority);
    }

    [Fact]
    public async SystemTask GetAllAsync_WithSorting_ShouldReturnSortedProjects()
    {
        var filter = new ProjectFilterDto
        {
            SortBy = "name",
            SortDescending = false
        };
        var result = await _service.GetAllAsync(filter);
        var list = result.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("Тестовый проект 1", list[0].Name);
        Assert.Equal("Тестовый проект 2", list[1].Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
