using CLINICA.Data; // Espacio de nombres de tu contexto de base de datos
using Microsoft.EntityFrameworkCore; // Uso de Entity Framework Core
using Microsoft.IdentityModel.Tokens;
using System.Text; // Para manejar texto y codificación

var builder = WebApplication.CreateBuilder(args);

// **Configuración de CORS**
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()        // Permitir cualquier origen
            .AllowAnyMethod()        // Permitir cualquier método HTTP
            .AllowAnyHeader();       // Permitir cualquier header HTTP
    });
});

// **Configuración de conexión a la base de datos**
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClinicaDbcontext>(options => options.UseSqlServer(connectionString!));

// **Agregar servicios necesarios**
builder.Services.AddControllers();  // Habilitar controladores
builder.Services.AddEndpointsApiExplorer(); // Documentación para API con Swagger
builder.Services.AddSwaggerGen();  // Generación de UI Swagger para pruebas

var app = builder.Build();

// **Habilitar Swagger solo en desarrollo**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // Redirección a HTTPS para mayor seguridad

app.UseRouting();           // Configurar rutas antes de CORS

app.UseCors("CorsPolicy");  // Aplicar la política de CORS configurada

app.UseAuthentication();    // Middleware de autenticación (si es necesario)
app.UseAuthorization();     // Middleware de autorización (si es necesario)

app.MapControllers();       // Mapear las rutas a los controladores

app.Run();                   // Ejecutar la aplicación
