using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Interfaces;

namespace ProjectManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> GetAll([FromQuery] TaskFilterDto? filter)
    {
        var tasks = await _taskService.GetAllAsync(filter);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound();
        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var task = await _taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var task = await _taskService.UpdateAsync(dto);
        if (task == null)
            return NotFound();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taskService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] string status)
    {
        var result = await _taskService.ChangeStatusAsync(id, status);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpGet("project/{projectId}")]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var tasks = await _taskService.GetTasksByProjectAsync(projectId);
        return Ok(tasks);
    }

    [HttpGet("executor/{executorId}")]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> GetByExecutor(int executorId)
    {
        var tasks = await _taskService.GetTasksByExecutorAsync(executorId);
        return Ok(tasks);
    }
}
