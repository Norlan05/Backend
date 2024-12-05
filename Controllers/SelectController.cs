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
        [HttpGet("GetAllReservations")]
        public IActionResult GetAllReservations()
        {
            try
            {
                // Obtener todos los registros de la tabla reservas
                var reservas = _db.Reservas
                    .Select(r => new
                    {
                        r.id,
                        r.nombre,
                        r.apellido,
                        r.correo_electronico,
                        r.numero_telefono,
                        fecha = r.fecha_hora.ToString("yyyy-MM-dd"),
                        hora = r.fecha_hora.ToString("HH:mm")
                    })
                    .ToList();

                // Verificar si la tabla tiene datos
                if (reservas == null || !reservas.Any())
                {
                    return NotFound(new { message = "No se encontraron reservas." });
                }

                // Retornar los datos en formato JSON
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                // Manejar posibles errores
                return StatusCode(500, new { message = "Error al obtener las reservas.", details = ex.Message });
            }
        }

        // Método para obtener una reserva específica por ID
        [HttpGet("GetReservationById/{id}")]
        public IActionResult GetReservationById(int id)
        {
            try
            {
                // Buscar un registro específico por ID
                var reserva = _db.Reservas
                    .Where(r => r.id == id)
                    .Select(r => new
                    {
                        r.id,
                        r.nombre,
                        r.apellido,
                        r.correo_electronico,
                        r.numero_telefono,
                        fecha = r.fecha_hora.ToString("yyyy-MM-dd"),
                        hora = r.fecha_hora.ToString("HH:mm")
                    })
                    .FirstOrDefault();

                // Validar si no se encontró la reserva
                if (reserva == null)
                {
                    return NotFound(new { message = "Reserva no encontrada." });
                }

                // Retornar el registro encontrado
                return Ok(reserva);
            }
            catch (Exception ex)
            {
                // Manejar posibles errores
                return StatusCode(500, new { message = "Error al obtener la reserva.", details = ex.Message });
            }
        }

        // Método para obtener reservas por correo electrónico
        [HttpGet("GetReservationsByEmail")]
        public IActionResult GetReservationsByEmail([FromQuery] string correo_electronico)
        {
            try
            {
                // Buscar las reservas filtradas por correo electrónico
                var reservas = _db.Reservas
                    .Where(r => r.correo_electronico == correo_electronico)
                    .Select(r => new
                    {
                        r.id,
                        r.nombre,
                        r.apellido,
                        r.correo_electronico,
                        r.numero_telefono,
                        fecha = r.fecha_hora.ToString("yyyy-MM-dd"),
                        hora = r.fecha_hora.ToString("HH:mm")

                    })
                    .ToList();

                // Verificar si se encontraron reservas
                if (reservas == null || !reservas.Any())
                {
                    return NotFound(new { message = "No se encontraron reservas para este correo electrónico." });
                }

                // Retornar los datos en formato JSON
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                // Manejar posibles errores
                return StatusCode(500, new { message = "Error al obtener las reservas por correo electrónico.", details = ex.Message });
            }
        }
    }
}
