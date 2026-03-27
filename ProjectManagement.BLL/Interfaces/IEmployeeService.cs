using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagement.BLL.DTOs;

namespace ProjectManagement.BLL.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<EmployeeDto>> SearchAsync(string searchTerm);
}
