using FreeLink.Configuration;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios ---
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configurar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab11 API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();  // Primero autenticación
app.UseAuthorization();   // Luego autorización

app.MapControllers();

app.Run();