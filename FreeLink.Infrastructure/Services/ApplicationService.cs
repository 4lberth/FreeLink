using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FreeLink.Application.Services;
using FreeLink.Application.UseCase.Application.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Infrastructure.Data.Context;

namespace FreeLink.Infrastructure.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly FreeLinkContext _db;
        private readonly INotificationService _notifier;

        public ApplicationService(FreeLinkContext db, INotificationService notifier)
        {
            _db = db;
            _notifier = notifier;
        }

        public async Task<ApplicationViewDto> SubmitApplicationAsync(int projectId, ApplicationCreateDto dto)
        {
            // 1. Validaciones básicas
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) throw new KeyNotFoundException("Proyecto no encontrado.");
            if (project.ProjectStatus != "Publicado") throw new InvalidOperationException("El proyecto no acepta postulaciones.");

            // Usamos _db.Projectapplications (tal como está en tu Context)
            bool exists = await _db.Projectapplications.AnyAsync(a => a.ProjectId == projectId && a.FreelancerId == dto.FreelancerId);
            if (exists) throw new InvalidOperationException("Ya te has postulado a este proyecto.");

            // 2. Crear entidad usando TU clase real: Projectapplication
            var app = new Projectapplication
            {
                ProjectId = projectId,
                FreelancerId = dto.FreelancerId,
                CoverLetter = dto.CoverLetter, 
                ProposedRate = dto.ProposedRate,
                EstimatedDuration = dto.EstimatedDuration,
                ApplicationStatus = "Pendiente",
                AppliedAt = DateTime.UtcNow
            };

            _db.Projectapplications.Add(app);
            await _db.SaveChangesAsync();

            // 3. Notificar
            await _notifier.SendNotificationAsync(project.ClientId, $"Nueva postulación para {project.Title}");

            // 4. Retornar DTO
            return MapToViewDto(app);
        }

        public async Task<IEnumerable<ApplicationViewDto>> GetApplicationsForProjectAsync(int projectId)
        {
            var apps = await _db.Projectapplications
                                .Where(a => a.ProjectId == projectId)
                                .ToListAsync();
            return apps.Select(MapToViewDto);
        }

        public async Task<IEnumerable<ApplicationViewDto>> GetApplicationsByFreelancerAsync(int freelancerId)
        {
            var apps = await _db.Projectapplications
                                .Where(a => a.FreelancerId == freelancerId)
                                .ToListAsync();
            return apps.Select(MapToViewDto);
        }

        // Método helper adaptado a tu entidad Projectapplication
        private static ApplicationViewDto MapToViewDto(Projectapplication app)
        {
            return new ApplicationViewDto
            {
                ApplicationId = app.ApplicationId,
                ProjectId = app.ProjectId,
                FreelancerId = app.FreelancerId,
                CoverLetter = app.CoverLetter ?? "",
                ProposedRate = app.ProposedRate,
                EstimatedDuration = app.EstimatedDuration,
                ApplicationStatus = app.ApplicationStatus ?? "Pendiente",
                AppliedAt = app.AppliedAt
            };
        }
    }
}