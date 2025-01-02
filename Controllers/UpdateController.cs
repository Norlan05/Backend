using CLINICA.Data;
using CLINICA.Model_request;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly ClinicaDbcontext _db;

        public UpdateController(ClinicaDbcontext db)
        {
            _db = db;
        }

        [HttpPost("Actualizar")]
        public IActionResult ActualizarEstadoReserva([FromBody] ActualizarEstadoDTO model)
        {
            // Validación de que se recibió un ID y estado
            if (string.IsNullOrEmpty(model.id))
            {
                return BadRequest("El id de la reserva es requerido.");
            }
            if (model.estado_id < 1 || model.estado_id > 3)
            {
                return BadRequest("El estado_id debe ser 1 (Pendiente), 2 (Confirmada) o 3 (Cancelada).");
            }

            // Buscar la reserva por su id
            var reserva = _db.Reservas.FirstOrDefault(r => r.id == int.Parse(model.id));

            if (reserva == null)
            {
                return NotFound("Reserva no encontrada.");
            }

            // Buscar el estado que se asignará según el estado_id
            var estado = _db.Estados.FirstOrDefault(e => e.estado_id == model.estado_id);

            if (estado == null)
            {
                return NotFound("Estado no encontrado.");
            }

            // Actualizar la reserva con el nuevo estado
            reserva.estado_id = estado.estado_id;
            reserva.estado_descripcion = estado.descripcion;

            // Guardar los cambios
            _db.SaveChanges();

            return Ok(new { mensaje = "Estado de la reserva actualizado correctamente." });
        }
    }
}
