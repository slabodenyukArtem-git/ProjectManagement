using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Interfaces;

namespace ProjectManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Director")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var employee = await _employeeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Director")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var employee = await _employeeService.UpdateAsync(dto);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Director")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _employeeService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpGet("search")]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var employees = await _employeeService.SearchAsync(term);
        return Ok(employees);
    }
}
