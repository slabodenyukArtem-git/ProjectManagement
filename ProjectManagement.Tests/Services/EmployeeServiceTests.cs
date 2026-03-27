using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL;
using ProjectManagement.DAL.Entities;
using Xunit;
using SystemTask = System.Threading.Tasks.Task;

namespace ProjectManagement.Tests.Services;

public class EmployeeServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _service = new EmployeeService(_context);
        
        _context.Employees.AddRange(
            new Employee { Id = 1, FirstName = "Иван", LastName = "Петров", Patronymic = "Иванович", Email = "ivan@test.com" },
            new Employee { Id = 2, FirstName = "Мария", LastName = "Сидорова", Patronymic = "Алексеевна", Email = "maria@test.com" },
            new Employee { Id = 3, FirstName = "Алексей", LastName = "Смирнов", Patronymic = "Дмитриевич", Email = "alexey@test.com" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async SystemTask GetAllAsync_ShouldReturnAllEmployees()
    {
        var result = await _service.GetAllAsync();
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async SystemTask GetByIdAsync_WithValidId_ShouldReturnEmployee()
    {
        var result = await _service.GetByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Иван", result.FirstName);
        Assert.Equal("Петров", result.LastName);
        Assert.Equal("ivan@test.com", result.Email);
    }

    [Fact]
    public async SystemTask GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async SystemTask CreateAsync_ShouldAddNewEmployee()
    {
        var dto = new CreateEmployeeDto
        {
            FirstName = "Новый",
            LastName = "Сотрудник",
            Patronymic = "Тестович",
            Email = "new@test.com"
        };
        var result = await _service.CreateAsync(dto);
        Assert.NotNull(result);
        Assert.Equal(dto.FirstName, result.FirstName);
        Assert.Equal(dto.LastName, result.LastName);
        Assert.Equal(dto.Email, result.Email);
    }

    [Fact]
    public async SystemTask UpdateAsync_WithValidId_ShouldUpdateEmployee()
    {
        var updateDto = new UpdateEmployeeDto
        {
            Id = 1,
            FirstName = "Обновленное",
            LastName = "Имя",
            Patronymic = "Обновленович",
            Email = "updated@test.com"
        };
        var result = await _service.UpdateAsync(updateDto);
        Assert.NotNull(result);
        Assert.Equal(updateDto.FirstName, result.FirstName);
        Assert.Equal(updateDto.LastName, result.LastName);
        Assert.Equal(updateDto.Email, result.Email);
    }

    [Fact]
    public async SystemTask UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        var updateDto = new UpdateEmployeeDto
        {
            Id = 999,
            FirstName = "Несуществующий",
            LastName = "Сотрудник",
            Email = "none@test.com"
        };
        var result = await _service.UpdateAsync(updateDto);
        Assert.Null(result);
    }

    [Fact]
    public async SystemTask DeleteAsync_WithValidId_ShouldRemoveEmployee()
    {
        var result = await _service.DeleteAsync(1);
        Assert.True(result);
        var deleted = await _context.Employees.FindAsync(1);
        Assert.Null(deleted);
    }

    [Fact]
    public async SystemTask DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _service.DeleteAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async SystemTask SearchAsync_WithSearchTerm_ShouldReturnMatchingEmployees()
    {
        var result = await _service.SearchAsync("Иван");
        Assert.Single(result);
        Assert.Contains("Иван", result.First().FirstName);
    }

    [Fact]
    public async SystemTask SearchAsync_WithEmptyTerm_ShouldReturnAllEmployees()
    {
        var result = await _service.SearchAsync("");
        Assert.Equal(3, result.Count());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
