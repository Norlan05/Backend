using CLINICA.Data;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public ReservationController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // Endpoint para actualizar solo el estado de la reserva
        [HttpPut("UpdateReservationState")]
        public async Task<IActionResult> UpdateReservationState([FromBody] EstadoRequest estadoRequest)
        {
            // Buscar la reserva en la base de datos por algún criterio, por ejemplo, nombre y correo
            var existingReservation = await _context.Reservas
                .FirstOrDefaultAsync(r => r.Estado == "Pendiente");  // O cualquier otro filtro necesario

            if (existingReservation == null)
            {
                return NotFound(new { message = "Reserva no encontrada." });
            }

            // Actualizar solo el estado de la reserva
            existingReservation.Estado = estadoRequest.Estado;

            try
            {
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                return Ok(new { message = "Estado de la reserva actualizado exitosamente." });
            }
            catch (DbUpdateException ex)
            {
                // Manejar errores en la actualización
                return StatusCode(500, new { message = "Error al actualizar el estado de la reserva.", details = ex.Message });
            }
        }
    }

    // Clase que solo contiene el estado para la actualización
    public class EstadoRequest
    {
        public string Estado { get; set; }
    }
}
