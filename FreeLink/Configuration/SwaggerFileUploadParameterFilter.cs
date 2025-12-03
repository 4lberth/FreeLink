using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FreeLink.Configuration;

/// Filtro para suprimir parámetros IFormFile en la generación de Swagger
public class SwaggerFileUploadParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        // Si el parámetro es IFormFile o List<IFormFile>, marcarlo como ignorado
        if (context.ApiParameterDescription.ModelMetadata?.ModelType == typeof(IFormFile) ||
            context.ApiParameterDescription.ModelMetadata?.ModelType == typeof(List<IFormFile>))
        {
            // Swagger debería ignorar este parámetro ya que será manejado por el OperationFilter
            parameter.Schema = null;
        }
    }
}