using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FreeLink.Configuration;

/// Filtro para manejar file uploads en Swagger
public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFileParameter = context.ApiDescription.ParameterDescriptions
            .Any(p => p.ModelMetadata?.ModelType == typeof(IFormFile) ||
                    p.ModelMetadata?.ModelType == typeof(List<IFormFile>));

        if (!hasFileParameter)
            return;

        // Obtener todos los parámetros de formulario
        var formParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Source.Id == "Form" ||
                    p.Source.Id == "FormFile" ||
                    p.ModelMetadata?.ModelType == typeof(IFormFile) ||
                    p.ModelMetadata?.ModelType == typeof(List<IFormFile>))
            .ToList();

        if (!formParameters.Any())
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
                        Properties = formParameters.ToDictionary(
                            p => p.Name,
                            p => {
                                if (p.ModelMetadata?.ModelType == typeof(IFormFile))
                                    return new OpenApiSchema { Type = "string", Format = "binary" };
                                else if (p.ModelMetadata?.ModelType == typeof(List<IFormFile>))
                                    return new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema { Type = "string", Format = "binary" }
                                    };
                                else
                                    return new OpenApiSchema { Type = "string" };
                            }
                        ),
                        Required = formParameters
                            .Where(p => p.IsRequired)
                            .Select(p => p.Name)
                            .ToHashSet()
                    }
                }
            }
        };

        // Limpiar parámetros que ahora están en el body
        var parametersToRemove = operation.Parameters
            .Where(p => formParameters.Any(fp => fp.Name == p.Name))
            .ToList();

        foreach (var param in parametersToRemove)
        {
            operation.Parameters.Remove(param);
        }
    }
}