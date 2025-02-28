using CLINICA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text; // Para manejar texto y codificación
using Microsoft.AspNetCore.Session; // Para trabajar con sesiones

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

// **Configuración de sesiones**
builder.Services.AddDistributedMemoryCache(); // Usar la memoria para la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de cerrar la sesión
    options.Cookie.HttpOnly = true; // La cookie solo puede ser leída por el servidor
    options.Cookie.IsEssential = true; // Asegúrate de que la cookie sea esencial para el funcionamiento de la app
});

var app = builder.Build();

// **Habilitar Swagger solo en desarrollo**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // Redirección a HTTPS para mayor seguridad

// **Habilitar middleware de sesiones**
app.UseSession(); // Esto es crucial para habilitar la sesión en tu aplicación

app.UseRouting();           // Configurar rutas antes de CORS

app.UseCors("CorsPolicy");  // Aplicar la política de CORS configurada

app.UseAuthentication();    // Middleware de autenticación (si es necesario)
app.UseAuthorization();     // Middleware de autorización (si es necesario)

app.MapControllers();       // Mapear las rutas a los controladores

app.Run();                   // Ejecutar la aplicación
