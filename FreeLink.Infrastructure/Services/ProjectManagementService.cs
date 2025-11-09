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

        private const string StatusPublicado = "Publicado";

        public ProjectManagementService(FreeLinkContext db, INotificationService notifier)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

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
                query = query.Where(p => (p.Title != null && p.Title.ToLower().Contains(s))
                                      || (p.Description != null && p.Description.ToLower().Contains(s))
                                      // Si Projectskills existe y quieres filtrar por la relación, ajusta esto según tu modelo.
                                      || (p.Projectskills != null && p.Projectskills.Any()));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim().ToLower();
                query = query.Where(p => (p.Title != null && p.Title.ToLower().Contains(t))
                                      || (p.Description != null && p.Description.ToLower().Contains(t)));
            }

            return await query.ToListAsync();
        }
    }
}