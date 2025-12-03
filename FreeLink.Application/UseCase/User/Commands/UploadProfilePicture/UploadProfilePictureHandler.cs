using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UploadProfilePicture;

public class UploadProfilePictureHandler : IRequestHandler<UploadProfilePictureCommand, UploadProfilePictureResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISupabaseStorageService _supabaseStorage;
    private static readonly string[] AllowedExtensions = { "jpg", "jpeg", "png", "webp" };
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    public UploadProfilePictureHandler(IUnitOfWork unitOfWork, ISupabaseStorageService supabaseStorage)
    {
        _unitOfWork = unitOfWork;
        _supabaseStorage = supabaseStorage;
    }

    public async Task<UploadProfilePictureResponse> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar archivo
            if (request.File == null || request.File.Length == 0)
            {
                return new UploadProfilePictureResponse
                {
                    Success = false,
                    Message = "Archivo no proporcionado"
                };
            }

            if (request.File.Length > MaxFileSize)
            {
                return new UploadProfilePictureResponse
                {
                    Success = false,
                    Message = "El archivo excede el tamaño máximo de 2 MB"
                };
            }

            var extension = Path.GetExtension(request.File.FileName).TrimStart('.').ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return new UploadProfilePictureResponse
                {
                    Success = false,
                    Message = "Solo se permiten imágenes: jpg, jpeg, png, webp"
                };
            }

            // 2. Obtener perfil del usuario
            var profiles = await _unitOfWork.Repository<Userprofile>()
                .GetAsync(p => p.UserId == request.UserId);
            
            var userProfile = profiles.FirstOrDefault();

            if (userProfile == null)
            {
                return new UploadProfilePictureResponse
                {
                    Success = false,
                    Message = "Perfil de usuario no encontrado"
                };
            }

            // 3. Eliminar foto anterior de Supabase si existe
            if (!string.IsNullOrEmpty(userProfile.ProfilePicture) &&
                userProfile.ProfilePicture.Contains("supabase"))
            {
                try
                {
                    var oldFileName = Path.GetFileName(new Uri(userProfile.ProfilePicture).LocalPath);
                    await _supabaseStorage.DeleteFileAsync($"user_{request.UserId}/{oldFileName}", "profiles");
                }
                catch
                {
                    // Ignorar error si no se puede eliminar foto anterior
                }
            }

            // 4. Subir nueva foto a Supabase en el bucket profiles
            using var stream = new MemoryStream();
            await request.File.CopyToAsync(stream);
            var fileBytes = stream.ToArray();

            var fileName = $"profile.{extension}";  // Nombre fijo para sobrescribir
            var publicUrl = await _supabaseStorage.UploadPdfAsync(
                fileBytes,
                fileName,
                "profiles",
                $"user_{request.UserId}"
            );

            // 5. Actualizar BD
            userProfile.ProfilePicture = publicUrl;
            await _unitOfWork.Repository<Userprofile>().Update(userProfile);
            await _unitOfWork.Complete();

            return new UploadProfilePictureResponse
            {
                Success = true,
                Message = "Foto de perfil actualizada exitosamente",
                ProfilePictureUrl = publicUrl
            };
        }
        catch (Exception ex)
        {
            return new UploadProfilePictureResponse
            {
                Success = false,
                Message = $"Error al subir foto de perfil: {ex.Message}"
            };
        }
    }
}
