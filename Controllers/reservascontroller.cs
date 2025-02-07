using CLINICA.Model_request;
using Microsoft.AspNetCore.Mvc;
using CLINICA.Modelos;
using CLINICA.Data;
using System.Linq;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public ReservasController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // Endpoint para buscar una reserva por correo o cédula
        [HttpGet("search")]
        public IActionResult SearchReservations([FromQuery] string? cedula, [FromQuery] string? correo)
        {
            try
            {
                // Obtener la fecha de hoy sin hora
                var fechaHoy = DateTime.Now.Date;

                // Inicializamos la consulta de reservas
                var reservas = _context.Reservas
                    .Where(r => r.fecha_hora.Date == fechaHoy)  // Filtramos las reservas para que sean solo del día de hoy
                    .AsQueryable();

                // Si se proporciona cédula, filtramos por cédula
                if (!string.IsNullOrWhiteSpace(cedula))
                {
                    reservas = reservas.Where(r => r.Cedula == cedula);
                }
                // Si se proporciona correo, filtramos por correo
                else if (!string.IsNullOrWhiteSpace(correo))
                {
                    reservas = reservas.Where(r => r.correo_electronico == correo);
                }

                // Realizamos la consulta y la obtenemos como lista
                var result = reservas
                    .Select(r => new
                    {
                        r.id,
                        // Concatenamos el nombre y apellido
                        nombre_completo = r.nombre + " " + r.apellido,
                        // Aseguramos que la cédula se muestre correctamente
                        Cedula = r.Cedula,
                        correo_electronico = r.correo_electronico,
                        numero_telefono = r.numero_telefono,
                        // Formateamos la fecha
                        fecha = r.fecha_hora.ToString("yyyy-MM-dd"), // Fecha en formato yyyy-MM-dd
                                                                     // Formateamos la hora para que sea de 12 horas con AM/PM
                        hora = r.fecha_hora.ToString("h:mm tt"),
                    })
                    .ToList();

                // Si no se encontraron resultados
                if (result.Count == 0)
                {
                    return NotFound("No se encontraron reservas para el criterio proporcionado.");
                }

                // Retornar los resultados
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al buscar las reservas.", details = ex.Message });
            }
        }

    }
}
