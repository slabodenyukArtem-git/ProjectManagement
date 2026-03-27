using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagement.BLL.DTOs;

namespace ProjectManagement.BLL.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync(ProjectFilterDto? filter = null);
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> AddExecutorAsync(int projectId, int employeeId);
    Task<bool> RemoveExecutorAsync(int projectId, int employeeId);
}
