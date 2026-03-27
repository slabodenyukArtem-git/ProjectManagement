using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagement.BLL.DTOs;

namespace ProjectManagement.BLL.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync(TaskFilterDto? filter = null);
    Task<TaskDto?> GetByIdAsync(int id);
    Task<TaskDto> CreateAsync(CreateTaskDto dto);
    Task<TaskDto?> UpdateAsync(UpdateTaskDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ChangeStatusAsync(int taskId, string newStatus);
    Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(int projectId);
    Task<IEnumerable<TaskDto>> GetTasksByExecutorAsync(int executorId);
}
