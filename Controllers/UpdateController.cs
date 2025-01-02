using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLINICA.Modelos;
using System.Threading.Tasks;
using CLINICA.Data;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadosController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public EstadosController(ClinicaDbcontext context)
        {
            _context = context;  // Asignamos el contexto a _context
        }

        [HttpPut("UpdateReservationStatusByCedula/{cedula}/{hora}")]
        public async Task<IActionResult> UpdateReservationStatusByCedula([FromRoute] string cedula, [FromRoute] string hora, [FromBody] int estadoId)
        {
            try
            {
                // Buscar la reserva que coincida con la cédula y la hora
                var reserva = await _context.Reservas
                    .Where(r => r.Cedula == cedula && r.fecha_hora.ToString("HH:mm") == hora)
                    .FirstOrDefaultAsync();

                if (reserva == null)
                {
                    return NotFound(new { message = "No se encontró una reserva con esa cédula y hora." });
                }

                // Verificar si el estado es válido
                var estado = await _context.Estados.FindAsync(estadoId);
                if (estado == null)
                {
                    return BadRequest(new { message = "Estado no válido." });
                }

                // Actualizar el estado de la reserva
                reserva.estado_id = estadoId;  // Asignar el nuevo estado ID
                reserva.estado_descripcion = estado.descripcion;  // Actualizar la descripción del estado

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok(new { message = "Estado actualizado correctamente." });
            }
            catch (Exception ex)
            {
                // Manejar posibles errores
                return StatusCode(500, new { message = "Error al actualizar el estado.", details = ex.Message });
            }
        }


    }
}
