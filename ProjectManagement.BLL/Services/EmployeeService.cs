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

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _context.Employees.ToListAsync();
        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Patronymic = e.Patronymic,
            Email = e.Email,
            FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}".Trim()
        });
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var e = await _context.Employees.FindAsync(id);
        if (e == null) return null;
        
        return new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Patronymic = e.Patronymic,
            Email = e.Email,
            FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}".Trim()
        };
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Patronymic = dto.Patronymic,
            Email = dto.Email
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Patronymic = employee.Patronymic,
            Email = employee.Email,
            FullName = $"{employee.LastName} {employee.FirstName} {employee.Patronymic}".Trim()
        };
    }

    public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(dto.Id);
        if (employee == null) return null;

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Patronymic = dto.Patronymic;
        employee.Email = dto.Email;

        await _context.SaveChangesAsync();

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Patronymic = employee.Patronymic,
            Email = employee.Email,
            FullName = $"{employee.LastName} {employee.FirstName} {employee.Patronymic}".Trim()
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTerm)
    {
        var employees = await _context.Employees
            .Where(e => e.FirstName.Contains(searchTerm) || 
                        e.LastName.Contains(searchTerm) || 
                        e.Email.Contains(searchTerm))
            .ToListAsync();
        
        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Patronymic = e.Patronymic,
            Email = e.Email,
            FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}".Trim()
        });
    }
}
