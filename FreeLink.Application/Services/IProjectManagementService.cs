using System.Collections.Generic;
using System.Threading.Tasks;
using FreeLink.Domain.Entities;
using FreeLink.Application.UseCase.Project.DTOs;

namespace FreeLink.Application.Services
{
    public interface IProjectManagementService
    {
        Task<Project?> CreateProjectAsync(ProjectCreateDto dto);
        Task<Project?> UpdateProjectAsync(int projectId, ProjectUpdateDto dto);
        Task<bool> DeleteProjectAsync(int projectId);
        Task<Project?> GetProjectAsync(int projectId);
        Task<IEnumerable<Project>> SearchProjectsAsync(string? skills = null, string? q = null);
    }
}