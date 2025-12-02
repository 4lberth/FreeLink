using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.UploadDeliverable;

public class UploadDeliverableHandler : IRequestHandler<UploadDeliverableCommand, UploadDeliverableResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;
    private static readonly string[] AllowedExtensions = { "pdf", "doc", "docx", "ppt", "pptx", "xls", "xlsx", "jpg", "jpeg", "png", "gif", "zip", "rar", "txt", "psd", "ai", "fig", "sketch" };
    private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB

    public UploadDeliverableHandler(IUnitOfWork unitOfWork, IFileService fileService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _notificationService = notificationService;
    }

    public async Task<UploadDeliverableResponse> Handle(UploadDeliverableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que haya archivos
            if (request.Files == null || !request.Files.Any())
            {
                return new UploadDeliverableResponse
                {
                    Success = false,
                    Message = "Debe proporcionar al menos un archivo"
                };
            }

            // 2. Validar título
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return new UploadDeliverableResponse
                {
                    Success = false,
                    Message = "El título del entregable es obligatorio"
                };
            }

            // 3. Obtener y validar el proyecto
            var project = await _unitOfWork.Repository<Project>().GetById(request.ProjectId);
            if (project == null)
            {
                return new UploadDeliverableResponse
                {
                    Success = false,
                    Message = "Proyecto no encontrado"
                };
            }

            // 4. Verificar permisos (solo el freelancer asignado puede subir entregables)
            if (project.AssignedFreelancerId != request.FreelancerId)
            {
                return new UploadDeliverableResponse
                {
                    Success = false,
                    Message = "Solo el freelancer asignado puede subir entregables"
                };
            }

            // 5. Validar estado del proyecto (debe estar "En Proceso")
            if (project.ProjectStatus != "En Proceso")
            {
                return new UploadDeliverableResponse
                {
                    Success = false,
                    Message = $"No se pueden subir entregables. El proyecto está en estado: {project.ProjectStatus}"
                };
            }

            // 6. Crear el entregable con estado inicial "Pendiente"
            var deliverable = new Projectdeliverable
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                DeliverableStatus = "Pendiente"
            };

            await _unitOfWork.Repository<Projectdeliverable>().Add(deliverable);
            await _unitOfWork.Complete();

            // 7. Procesar y subir archivos
            int uploadedFilesCount = 0;
            foreach (var file in request.Files)
            {
                // Validar archivo
                if (!_fileService.ValidateFile(file, AllowedExtensions, MaxFileSize))
                {
                    continue; // Saltar archivos inválidos
                }

                // Subir archivo
                var folder = $"deliverables/project_{request.ProjectId}/deliverable_{deliverable.DeliverableId}";
                var fileUrl = await _fileService.SaveFileAsync(file, folder);

                // Crear registro de archivo
                var deliverableFile = new Deliverablefile
                {
                    DeliverableId = deliverable.DeliverableId,
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                    FileType = _fileService.GetFileExtension(file.FileName),
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Deliverablefile>().Add(deliverableFile);
                uploadedFilesCount++;
            }

            if (uploadedFilesCount == 0)
            {
                // Si no se subió ningún archivo válido, eliminar el entregable
                await _unitOfWork.Repository<Projectdeliverable>().Delete(deliverable.DeliverableId);
                await _unitOfWork.Complete();

                return new UploadDeliverableResponse
                {
                    Success = false,
                    Message = "No se pudo subir ningún archivo válido. Verifica que los archivos cumplan con los requisitos de tamaño y tipo"
                };
            }

            await _unitOfWork.Complete();

            // 8. Crear notificación para el cliente
            await _notificationService.CreateNotificationAsync(
                userId: project.ClientId,
                type: "Deliverable",
                title: "Nuevo entregable subido",
                message: $"El freelancer ha subido un nuevo entregable en el proyecto: {project.Title}",
                resourceType: "Deliverable",
                resourceId: deliverable.DeliverableId
            );

            // 9. Registrar actividad en el proyecto
            await _notificationService.LogProjectActivityAsync(
                projectId: request.ProjectId,
                userId: request.FreelancerId,
                activityType: "DeliverableUploaded",
                description: $"Entregable '{request.Title}' subido con {uploadedFilesCount} archivo(s)"
            );

            return new UploadDeliverableResponse
            {
                Success = true,
                Message = $"Entregable subido exitosamente con {uploadedFilesCount} archivo(s)",
                DeliverableId = deliverable.DeliverableId
            };
        }
        catch (Exception ex)
        {
            return new UploadDeliverableResponse
            {
                Success = false,
                Message = $"Error al subir entregable: {ex.Message}"
            };
        }
    }
}
