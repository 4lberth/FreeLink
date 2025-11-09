using FreeLink.Configuration;
using FreeLink.Infrastructure.Configuration;
using Microsoft.Extensions.FileProviders; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);


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

app.UseStaticFiles();


var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? app.Environment.ContentRootPath, "uploads");


if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath), 
    RequestPath = "/uploads"
});


app.UseAuthentication();  
app.UseAuthorization();   

app.MapControllers();

app.Run();