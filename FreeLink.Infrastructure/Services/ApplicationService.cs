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
            var project = await _db.Projects.FindAsync(projectId);
            if (project == null) throw new KeyNotFoundException("Proyecto no encontrado.");
            if (project.ProjectStatus != "Publicado") throw new InvalidOperationException("El proyecto no acepta postulaciones en su estado actual.");

            bool exists = await _db.Projectapplications.AnyAsync(a => a.ProjectId == projectId && a.FreelancerId == dto.FreelancerId);
            if (exists) throw new InvalidOperationException("Ya te has postulado a este proyecto.");

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

            await _notifier.SendNotificationAsync(project.ClientId, $"Nueva postulación recibida para el proyecto '{project.Title}'");

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

        public async Task<bool> AcceptApplicationAsync(int applicationId)
        {
            // Necesitamos incluir el Proyecto para verificar su estado y actualizarlo
            var app = await _db.Projectapplications
                               .Include(a => a.Project)
                               .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (app == null) return false;

            // Validar que el proyecto aún esté "Publicado" antes de asignar
            if (app.Project.ProjectStatus != "Publicado")
            {
                throw new InvalidOperationException("No se puede aceptar la postulación porque el proyecto ya no está disponible.");
            }

            // 1. Marcar esta postulación como Aceptada
            app.ApplicationStatus = "Aceptada";
            app.RespondedAt = DateTime.UtcNow;

            // 2. Actualizar el estado del proyecto y asignar al freelancer
            app.Project.ProjectStatus = "Asignado";
            app.Project.AssignedFreelancerId = app.FreelancerId;

            // 3. Rechazar automáticamente el resto de postulaciones de este proyecto
            var otherApps = await _db.Projectapplications
                                     .Where(a => a.ProjectId == app.ProjectId && a.ApplicationId != applicationId)
                                     .ToListAsync();
            
            foreach (var other in otherApps)
            {
                other.ApplicationStatus = "Rechazada";
                other.RespondedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            // Notificar al freelancer seleccionado
            await _notifier.SendNotificationAsync(app.FreelancerId, $"¡Felicidades! Tu postulación para '{app.Project.Title}' ha sido aceptada.");

            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId)
        {
            var app = await _db.Projectapplications.FindAsync(applicationId);
            if (app == null) return false;
            
            if (app.ApplicationStatus != "Pendiente")
            {
            }

            app.ApplicationStatus = "Rechazada";
            app.RespondedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

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