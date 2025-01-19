using CLINICA.Data; // Espacio de nombres de tu contexto de base de datos
using Microsoft.EntityFrameworkCore; // Uso de Entity Framework Core
using Microsoft.IdentityModel.Tokens;
using System.Text; // Para manejar texto y codificaci�n

var builder = WebApplication.CreateBuilder(args);

// **Configuraci�n de CORS**
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()        // Permitir cualquier origen
            .AllowAnyMethod()        // Permitir cualquier m�todo HTTP
            .AllowAnyHeader();       // Permitir cualquier header HTTP
    });
});

// **Configuraci�n de conexi�n a la base de datos**
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClinicaDbcontext>(options => options.UseSqlServer(connectionString!));

// **Agregar servicios necesarios**
builder.Services.AddControllers();  // Habilitar controladores
builder.Services.AddEndpointsApiExplorer(); // Documentaci�n para API con Swagger
builder.Services.AddSwaggerGen();  // Generaci�n de UI Swagger para pruebas

var app = builder.Build();

// **Habilitar Swagger solo en desarrollo**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // Redirecci�n a HTTPS para mayor seguridad

app.UseRouting();           // Configurar rutas antes de CORS

app.UseCors("CorsPolicy");  // Aplicar la pol�tica de CORS configurada

app.UseAuthentication();    // Middleware de autenticaci�n (si es necesario)
app.UseAuthorization();     // Middleware de autorizaci�n (si es necesario)

app.MapControllers();       // Mapear las rutas a los controladores

app.Run();                   // Ejecutar la aplicaci�n
