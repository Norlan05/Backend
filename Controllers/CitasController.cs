using CLINICA.Data;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization; // Asegúrate de incluir esta línea para trabajar con cultura y fechas
using System.Linq;
using System.Threading.Tasks;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public CitasController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // Endpoint para obtener citas confirmadas agrupadas por mes
        [HttpGet("citasPorMes")]
        public async Task<IActionResult> GetCitasPorMes()
        {
            // Consultar las citas agrupadas por mes y año
            var citasPorMes = await _context.Reservas
                .Where(r => r.estado_descripcion == "Confirmada") // Solo citas confirmadas
                .GroupBy(r => new { r.fecha.Year, r.fecha.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Citas = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            // Formatear el resultado en el lado del cliente
            var formattedCitasPorMes = citasPorMes.Select(g => new
            {
                // Convertir el número del mes a su nombre
                Mes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month),
                g.Citas
            }).ToList();

            // Si no hay citas, retornamos un mensaje adecuado
            if (formattedCitasPorMes.Count == 0)
            {
                return NotFound("No hay citas confirmadas disponibles.");
            }

            return Ok(formattedCitasPorMes);
        }
    }
}
