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
        
        // ... en IProjectManagementService.cs ...
        Task<bool> StartProjectAsync(int projectId);
        Task<bool> CompleteProjectAsync(int projectId);
        Task<bool> CancelProjectAsync(int projectId);

        // Mensajería con adjuntos
        Task<Projectmessage> SendMessageAsync(int projectId, int senderId, string messageText, IEnumerable<FileUploadRequest> files);
        Task<IEnumerable<Projectmessage>> GetMessagesAsync(int projectId);
        Task<bool> MarkMessageAsReadAsync(int messageId, int userId);

        // Entregables
        Task<Projectdeliverable?> UploadDeliverableAsync(int projectId, int freelancerId, ProjectDeliverableCreateDto dto, IEnumerable<FileUploadRequest> files);
        Task<Projectdeliverable?> ReviewDeliverableAsync(int deliverableId, int reviewerId, string decision, string? comments);
        Task<IEnumerable<Projectdeliverable>> GetDeliverablesAsync(int projectId);
        Task<IEnumerable<Projectactivitylog>> GetActivityAsync(int projectId);
        Task<DeliverableStatusSummaryDto> GetDeliverablesSummaryAsync(int projectId);
    }
}