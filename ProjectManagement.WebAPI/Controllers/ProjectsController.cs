using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.DTOs;
using ProjectManagement.BLL.Interfaces;

namespace ProjectManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> GetAll([FromQuery] ProjectFilterDto? filter)
    {
        var projects = await _projectService.GetAllAsync(filter);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Director,ProjectManager,Employee")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
            return NotFound();
        return Ok(project);
    }

    [HttpPost]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var project = await _projectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var project = await _projectService.UpdateAsync(dto);
        if (project == null)
            return NotFound();
        return Ok(project);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Director")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _projectService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("{projectId}/executors/{employeeId}")]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> AddExecutor(int projectId, int employeeId)
    {
        var result = await _projectService.AddExecutorAsync(projectId, employeeId);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{projectId}/executors/{employeeId}")]
    [Authorize(Roles = "Director,ProjectManager")]
    public async Task<IActionResult> RemoveExecutor(int projectId, int employeeId)
    {
        var result = await _projectService.RemoveExecutorAsync(projectId, employeeId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
