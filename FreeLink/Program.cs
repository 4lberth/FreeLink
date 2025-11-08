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



var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");

// 2. ¡La corrección clave!

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Configurar archivos estáticos para servir wwwroot (si existe)
app.UseStaticFiles();

// Configurar ruta personalizada para uploads
app.UseStaticFiles(new StaticFileOptions
{
    // 3. Usar nuestra variable 'uploadsPath' que ya hemos verificado
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();  
app.UseAuthorization();  
app.MapControllers();

app.Run();