using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FreeLink.Configuration;

/// <summary>
/// Filtro para manejar file uploads en Swagger
/// </summary>
public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFileParameter = context.ApiDescription.ParameterDescriptions
            .Any(p => p.ModelMetadata?.ModelType == typeof(IFormFile));

        if (!hasFileParameter)
            return;

        // Asegurar que el endpoint tenga el content-type correcto
        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = context.ApiDescription.ParameterDescriptions
                            .Where(p => p.Source.Id == "Form" || p.Source.Id == "FormFile")
                            .ToDictionary(
                                p => p.Name,
                                p => p.ModelMetadata?.ModelType == typeof(IFormFile)
                                    ? new OpenApiSchema { Type = "string", Format = "binary" }
                                    : new OpenApiSchema { Type = "string" }
                            ),
                        Required = context.ApiDescription.ParameterDescriptions
                            .Where(p => p.IsRequired && (p.Source.Id == "Form" || p.Source.Id == "FormFile"))
                            .Select(p => p.Name)
                            .ToHashSet()
                    }
                }
            }
        };

        // Limpiar parámetros que ahora están en el body
        var parametersToRemove = operation.Parameters
            .Where(p => context.ApiDescription.ParameterDescriptions
                .Any(pd => pd.Name == p.Name && (pd.Source.Id == "Form" || pd.Source.Id == "FormFile")))
            .ToList();

        foreach (var param in parametersToRemove)
        {
            operation.Parameters.Remove(param);
        }
    }
}
