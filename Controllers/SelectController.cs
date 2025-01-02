using CLINICA.Model_request;
using Microsoft.AspNetCore.Mvc;
using CLINICA.Modelos;
using CLINICA.Data;
using System.Linq;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SelectController : ControllerBase
    {
        private readonly ClinicaDbcontext _db;

        // Constructor para inyectar el contexto de la base de datos
        public SelectController(ClinicaDbcontext db)
        {
            _db = db;
        }

        // Método para obtener todas las reservas
        [HttpGet("GetReservas")]
        public IActionResult GetAllReservations()
        {
            try
            {
                // Obtener la fecha actual
                var fechaActual = DateTime.Now.Date; // Fecha sin hora

                // Obtener solo las reservas del día de hoy o posteriores
                var reservas = _db.Reservas
                    .Where(r => r.fecha_hora.Date >= fechaActual)  // Filtramos las reservas
                    .Select(r => new
                    {
                        r.id,
                        r.nombre,
                        r.apellido,
                        r.correo_electronico,
                        r.numero_telefono,
                        fecha = r.fecha_hora.ToString("yyyy-MM-dd"),
                        hora = r.fecha_hora.ToString("HH:mm"),
                        r.Cedula,
                        estado = r.estado_descripcion // Solo la descripción del estado, no el ID
                    })
                    .ToList();

                // Verificar si hay reservas disponibles
                if (!reservas.Any())
                {
                    return NotFound(new { message = "No se encontraron reservas para hoy o en el futuro." });
                }

                // Retornar las reservas filtradas en formato JSON
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                // Manejar posibles errores
                return StatusCode(500, new { message = "Error al obtener las reservas.", details = ex.Message });
            }
        }
    }
}
