using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FreeLink.Application.Services;
using FreeLink.Application.UseCase.Project.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Infrastructure.Data.Context;
using FreeLink.Domain.Ports;

namespace FreeLink.Infrastructure.Services
{
    public class ProjectManagementService : IProjectManagementService
    {
        private readonly FreeLinkContext _db;
        private readonly INotificationService _notifier;
        private readonly IFileStorageService _fileStorage;

        // Constantes para estados del proyecto (deben coincidir con tu ENUM en DB)
        private const string StatusPublicado = "Publicado";
        private const string StatusAsignado = "Asignado";
        private const string StatusEnProceso = "En Proceso";
        private const string StatusCompletado = "Completado";
        private const string StatusCancelado = "Cancelado";

        public ProjectManagementService(FreeLinkContext db, INotificationService notifier, IFileStorageService fileStorage)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        // --- MÉTODOS CRUD EXISTENTES (COMMIT 1) ---

        public async Task<Project?> CreateProjectAsync(ProjectCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Title is required");
            if (dto.Budget < 0) throw new ArgumentException("Budget must be >= 0");
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.DeadlineDate <= today) throw new ArgumentException("DeadlineDate must be in the future");

            var project = new Project
            {
                ClientId = dto.ClientId,
                Title = dto.Title,
                Description = dto.Description,
                Budget = dto.Budget,
                DeadlineDate = dto.DeadlineDate,
                ProjectStatus = StatusPublicado,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            await _notifier.SendNotificationAsync(project.ClientId, $"Proyecto '{project.Title}' publicado.");

            return project;
        }

        public async Task<Project?> UpdateProjectAsync(int projectId, ProjectUpdateDto dto)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Title)) project.Title = dto.Title!;
            if (!string.IsNullOrWhiteSpace(dto.Description)) project.Description = dto.Description!;
            if (dto.Budget.HasValue)
            {
                if (dto.Budget.Value < 0) throw new ArgumentException("Budget must be >= 0");
                project.Budget = dto.Budget.Value;
            }
            if (dto.DeadlineDate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (dto.DeadlineDate.Value <= today) throw new ArgumentException("DeadlineDate must be in the future");
                project.DeadlineDate = dto.DeadlineDate.Value;
            }

            project.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await _notifier.SendNotificationAsync(project.ClientId, $"Proyecto '{project.Title}' actualizado.");

            return project;
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            // Usamos Projectapplications porque así se llama en tu Context
            var project = await _db.Projects
                .Include(p => p.Projectapplications) 
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null) return false;

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();

            await _notifier.SendNotificationAsync(project.ClientId, $"Proyecto '{project.Title}' eliminado.");
            return true;
        }

        public async Task<Project?> GetProjectAsync(int projectId)
        {
            return await _db.Projects
                .Include(p => p.Projectapplications)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string? skills = null, string? q = null)
        {
            var query = _db.Projects.AsQueryable();

            
            query = query.Where(p => p.ProjectStatus == StatusPublicado);

            if (!string.IsNullOrWhiteSpace(skills))
            {
                var s = skills.Trim().ToLower();
                query = query.Where(p => (p.Title.ToLower().Contains(s))
                                      || (p.Description.ToLower().Contains(s))
                                      );
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim().ToLower();
                query = query.Where(p => (p.Title.ToLower().Contains(t))
                                      || (p.Description.ToLower().Contains(t)));
            }

            return await query.ToListAsync();
        }
        
        public async Task<bool> StartProjectAsync(int projectId)
        {
            var project = await _db.Projects.FindAsync(projectId);

            if (project == null || project.ProjectStatus != StatusAsignado)
            {
                return false; 
            }

            project.ProjectStatus = StatusEnProceso;
            project.StartDate = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Notificaciones
            await _notifier.SendNotificationAsync(project.ClientId, $"El proyecto '{project.Title}' ha comenzado.");
            if (project.AssignedFreelancerId.HasValue)
            {
                 await _notifier.SendNotificationAsync(project.AssignedFreelancerId.Value, $"El proyecto '{project.Title}' ha comenzado.");
            }

            return true;
        }

        public async Task<bool> CompleteProjectAsync(int projectId)
        {
            var project = await _db.Projects.FindAsync(projectId);

            // Solo se puede completar si está en proceso
            if (project == null || project.ProjectStatus != StatusEnProceso)
            {
                return false;
            }

            project.ProjectStatus = StatusCompletado;
            project.CompletionDate = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _notifier.SendNotificationAsync(project.ClientId, $"El proyecto '{project.Title}' ha sido marcado como completado.");
            if (project.AssignedFreelancerId.HasValue)
            {
                await _notifier.SendNotificationAsync(project.AssignedFreelancerId.Value, $"¡Felicidades! Has completado el proyecto '{project.Title}'.");
            }

            return true;
        }

        public async Task<bool> CancelProjectAsync(int projectId)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return false;

            // Se puede cancelar si no ha sido completado aún
            if (project.ProjectStatus == StatusCompletado || project.ProjectStatus == StatusCancelado)
            {
                return false;
            }

            project.ProjectStatus = StatusCancelado;
            project.UpdatedAt = DateTime.UtcNow;
            
            await _db.SaveChangesAsync();

            await _notifier.SendNotificationAsync(project.ClientId, $"El proyecto '{project.Title}' ha sido cancelado.");
            if (project.AssignedFreelancerId.HasValue)
            {
                await _notifier.SendNotificationAsync(project.AssignedFreelancerId.Value, $"El proyecto '{project.Title}' ha sido cancelado por el cliente.");
            }

            return true;
        }

        // === MENSAJERÍA CON ADJUNTOS ===
        private async Task<Project?> GetAuthorizedProject(int projectId, int userId)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return null;
            if (project.ClientId != userId && project.AssignedFreelancerId != userId) return null;
            return project;
        }

        public async Task<Projectmessage> SendMessageAsync(int projectId, int senderId, string messageText, IEnumerable<FileUploadRequest> files)
        {
            var project = await GetAuthorizedProject(projectId, senderId);
            if (project == null) throw new UnauthorizedAccessException("No autorizado para este proyecto.");

            var message = new Projectmessage
            {
                ProjectId = projectId,
                SenderId = senderId,
                MessageText = messageText,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            _db.Projectmessages.Add(message);
            await _db.SaveChangesAsync();

            foreach (var f in files ?? Enumerable.Empty<FileUploadRequest>())
            {
                var storedName = await _fileStorage.SaveFileAsync(f.Stream, f.FileName, $"projects/{projectId}/messages");
                var url = _fileStorage.GetFileUrl(storedName, $"projects/{projectId}/messages");
                var attach = new Messageattachment
                {
                    MessageId = message.MessageId,
                    FileName = f.FileName,
                    FileUrl = url,
                    FileType = f.ContentType,
                    FileSize = f.Length,
                    UploadedAt = DateTime.UtcNow
                };
                _db.Messageattachments.Add(attach);
            }
            await _db.SaveChangesAsync();

            _db.Projectactivitylogs.Add(new Projectactivitylog
            {
                ProjectId = projectId,
                UserId = senderId,
                ActivityType = "Mensaje",
                ActivityDescription = "Mensaje enviado",
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return await _db.Projectmessages
                .Include(m => m.Messageattachments)
                .FirstAsync(m => m.MessageId == message.MessageId);
        }

        public async Task<IEnumerable<Projectmessage>> GetMessagesAsync(int projectId)
        {
            return await _db.Projectmessages
                .Include(m => m.Messageattachments)
                .Where(m => m.ProjectId == projectId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        // === ENTREGABLES ===
        private const string EntregableEnviado = "Enviado";

        public async Task<Projectdeliverable?> UploadDeliverableAsync(int projectId, int freelancerId, ProjectDeliverableCreateDto dto, IEnumerable<FileUploadRequest> files)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) return null;
            if (project.AssignedFreelancerId != freelancerId) throw new UnauthorizedAccessException("Solo el freelancer asignado puede subir entregables.");

            var deliverable = new Projectdeliverable
            {
                ProjectId = projectId,
                Title = dto.Title,
                Description = dto.Description,
                DeliverableStatus = EntregableEnviado,
                SubmittedAt = DateTime.UtcNow,
                DueDate = dto.DueDate
            };
            _db.Projectdeliverables.Add(deliverable);
            await _db.SaveChangesAsync();

            foreach (var f in files ?? Enumerable.Empty<FileUploadRequest>())
            {
                var storedName = await _fileStorage.SaveFileAsync(f.Stream, f.FileName, $"projects/{projectId}/deliverables/{deliverable.DeliverableId}");
                var url = _fileStorage.GetFileUrl(storedName, $"projects/{projectId}/deliverables/{deliverable.DeliverableId}");
                var df = new Deliverablefile
                {
                    DeliverableId = deliverable.DeliverableId,
                    FileName = f.FileName,
                    FileUrl = url,
                    FileType = f.ContentType,
                    FileSize = f.Length,
                    UploadedAt = DateTime.UtcNow
                };
                _db.Deliverablefiles.Add(df);
            }
            await _db.SaveChangesAsync();

            _db.Projectactivitylogs.Add(new Projectactivitylog
            {
                ProjectId = projectId,
                UserId = freelancerId,
                ActivityType = "Entregable",
                ActivityDescription = $"Entregable '{deliverable.Title}' enviado",
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return await _db.Projectdeliverables
                .Include(d => d.Deliverablefiles)
                .FirstAsync(d => d.DeliverableId == deliverable.DeliverableId);
        }

        private const string EntregableEnRevision = "En revisión";
        private const string EntregableAprobado = "Aprobado";
        private const string EntregableRechazado = "Rechazado";

        public async Task<Projectdeliverable?> ReviewDeliverableAsync(int deliverableId, int reviewerId, string decision, string? comments)
        {
            var deliverable = await _db.Projectdeliverables
                .Include(d => d.Project)
                .FirstOrDefaultAsync(d => d.DeliverableId == deliverableId);
            if (deliverable == null) return null;

            var project = deliverable.Project ?? await _db.Projects.FindAsync(deliverable.ProjectId);
            if (project == null) return null;
            if (project.ClientId != reviewerId) throw new UnauthorizedAccessException("Solo el cliente del proyecto puede revisar entregables.");

            var normalized = (decision ?? string.Empty).Trim().ToLowerInvariant();
            string newStatus = normalized switch
            {
                "approve" or "approved" or "aprobado" => EntregableAprobado,
                "reject" or "rejected" or "rechazado" => EntregableRechazado,
                "review" or "enrevision" => EntregableEnRevision,
                _ => EntregableEnRevision
            };

            deliverable.DeliverableStatus = newStatus;
            deliverable.ReviewComments = comments;
            deliverable.ReviewedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _db.Projectactivitylogs.Add(new Projectactivitylog
            {
                ProjectId = deliverable.ProjectId,
                UserId = reviewerId,
                ActivityType = "Revisión",
                ActivityDescription = $"Entregable '{deliverable.Title}' marcado como {newStatus}",
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return deliverable;
        }

        public async Task<IEnumerable<Projectdeliverable>> GetDeliverablesAsync(int projectId)
        {
            return await _db.Projectdeliverables
                .Include(d => d.Deliverablefiles)
                .Where(d => d.ProjectId == projectId)
                .OrderByDescending(d => d.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projectactivitylog>> GetActivityAsync(int projectId)
        {
            return await _db.Projectactivitylogs
                .Where(a => a.ProjectId == projectId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<DeliverableStatusSummaryDto> GetDeliverablesSummaryAsync(int projectId)
        {
            var statuses = await _db.Projectdeliverables
                .Where(d => d.ProjectId == projectId)
                .Select(d => d.DeliverableStatus)
                .ToListAsync();

            return new DeliverableStatusSummaryDto
            {
                Pending = statuses.Count(s => string.Equals(s, "Pendiente", StringComparison.OrdinalIgnoreCase)),
                Sent = statuses.Count(s => string.Equals(s, EntregableEnviado, StringComparison.OrdinalIgnoreCase)),
                InReview = statuses.Count(s => string.Equals(s, EntregableEnRevision, StringComparison.OrdinalIgnoreCase)),
                Approved = statuses.Count(s => string.Equals(s, EntregableAprobado, StringComparison.OrdinalIgnoreCase)),
                Rejected = statuses.Count(s => string.Equals(s, EntregableRechazado, StringComparison.OrdinalIgnoreCase))
            };
        }
    }
}