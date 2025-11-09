using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FreeLink.Application.Services;
using FreeLink.Application.UseCase.Project.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Infrastructure.Data.Context;

namespace FreeLink.Infrastructure.Services
{
    public class ProjectManagementService : IProjectManagementService
    {
        private readonly FreeLinkContext _db;
        private readonly INotificationService _notifier;

        // Constantes para estados del proyecto (deben coincidir con tu ENUM en DB)
        private const string StatusPublicado = "Publicado";
        private const string StatusAsignado = "Asignado";
        private const string StatusEnProceso = "En Proceso";
        private const string StatusCompletado = "Completado";
        private const string StatusCancelado = "Cancelado";

        public ProjectManagementService(FreeLinkContext db, INotificationService notifier)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
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
    }
}