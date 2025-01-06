using CLINICA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuscarPacienteController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public BuscarPacienteController(ClinicaDbcontext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // API para buscar paciente por cédula y obtener el historial de consultas
        [HttpGet("buscar")]
        public async Task<IActionResult> GetPacienteHistorial(string cedula)
        {
            // Buscar al paciente por cédula
            var paciente = await _context.Pacientes
                .Where(p => p.Cedula == cedula)
                .Select(p => new
                {
                    Nombre = p.nombre + " " + p.apellido,  // Concatenar nombre y apellido
                    p.Cedula,
                    p.Direccion,
                    Fecha_Nacimiento = p.Fecha_Nacimiento.ToString("yyyy-MM-dd"),  // Formato de fecha
                    Consultas = _context.Reservas
                        .Where(r => r.Cedula == p.Cedula)
                        .Join(_context.Consultas, r => r.id, c => c.ReservaID, (r, c) => new
                        {
                            c.Motivo_Consulta,
                            c.Diagnostico,
                            c.Observaciones,
                            Fecha_Consulta = r.fecha.ToString("yyyy-MM-dd")  // Formato de fecha
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            // Si no se encuentra el paciente, retornar un error
            if (paciente == null)
            {
                return NotFound(new { message = "Paciente no encontrado o sin historial de consultas" });
            }

            // Retornar la información del paciente y su historial de consultas
            return Ok(paciente);
        }
    }
}
