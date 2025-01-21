using CLINICA.Data;
using CLINICA.Model_request;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public DashboardController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // GET: api/dashboard
        [HttpGet]
        public async Task<ActionResult<DashboardData>> GetDashboardData()
        {
            // Total de pacientes registrados
            var totalPacientes = await _context.Pacientes.CountAsync();

            // Total de citas canceladas
            var citasCanceladas = await _context.Reservas.Where(c => c.estado_descripcion == "Cancelada").CountAsync();

            // Total de citas atendidas
            var citasAtendidas = await _context.Reservas.Where(c => c.estado_descripcion == "Confirmada").CountAsync();

            // Total de citas pendientes
            var citasPendientes = await _context.Reservas.Where(c => c.estado_descripcion == "Pendiente").CountAsync();

            // Crear el objeto para enviar los datos
            var dashboardData = new DashboardData
            {
                TotalPacientes = totalPacientes,
                CitasCanceladas = citasCanceladas,
                CitasAtendidas = citasAtendidas,
                CitasPendientes = citasPendientes
            };

            return Ok(dashboardData);
        }
    }

    public class DashboardData
    {
        public int TotalPacientes { get; set; }
        public int CitasCanceladas { get; set; }
        public int CitasAtendidas { get; set; }
        public int CitasPendientes { get; set; }
    }
}
