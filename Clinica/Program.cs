using Clinica_Dr_Toruño.data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()        // Permitir cualquier origen
            .AllowAnyMethod()        // Permitir cualquier m?todo HTTP
            .AllowAnyHeader();       // Permitir cualquier header HTTP
    });
});

// Add services to the container.
var connection_string = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClinicaDbcontext>(options => options.UseSqlServer(connection_string!));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();