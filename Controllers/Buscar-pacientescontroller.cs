using CLINICA.Data;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Buscar_pacientes_cs : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        // Constructor para inyectar el contexto de la base de datos
        public Buscar_pacientes_cs(ClinicaDbcontext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Endpoint para buscar pacientes por cédula o correo electrónico
        [HttpGet("search")]
        public IActionResult SearchPacientes([FromQuery] string? cedula, [FromQuery] string? correo)
        {
            try
            {
                // Inicializamos la consulta de pacientes
                var pacientes = _context.Pacientes.AsQueryable();

                // Si se proporciona cédula, filtramos por cédula
                if (!string.IsNullOrWhiteSpace(cedula))
                {
                    pacientes = pacientes.Where(p => p.Cedula == cedula);
                }
                // Si se proporciona correo, filtramos por correo electrónico
                else if (!string.IsNullOrWhiteSpace(correo))
                {
                    pacientes = pacientes.Where(p => p.correo_electronico == correo);
                }

                // Realizamos la consulta y la obtenemos como lista
                var result = pacientes
                    .Select(p => new
                    {
                        p.PacienteID,               // ID único del paciente
                        p.nombre,                   // Nombre del paciente
                        p.apellido,                 // Apellido del paciente
                        p.correo_electronico,       // Correo electrónico del paciente
                        p.Telefono,                 // Teléfono del paciente
                        p.Cedula,                   // Cédula del paciente
                        p.Direccion,                // Dirección del paciente
                        Fecha_Nacimiento = p.Fecha_Nacimiento.ToString("yyyy-MM-dd"), // Fecha de nacimiento en formato "yyyy-MM-dd"
                    })
                    .ToList();

                // Si no se encontraron resultados
                if (result.Count == 0)
                {
                    return NotFound("No se encontraron pacientes para el criterio proporcionado.");
                }

                // Retornar los resultados
                return Ok(result);
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos un mensaje de error con detalles
                return StatusCode(500, new { message = "Error interno al buscar los pacientes.", details = ex.Message });
            }
        }
    }
}
