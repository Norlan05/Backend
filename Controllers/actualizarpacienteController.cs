using CLINICA.Data;
using CLINICA.Model_request;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActualizarPacienteController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public ActualizarPacienteController(ClinicaDbcontext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Endpoint para actualizar un paciente
        [HttpPut("update")]
        public IActionResult UpdatePaciente([FromBody] ActualizarPacienteDTO pacienteDTO)
        {
            try
            {
                // Buscar el paciente por su ID
                var pacienteExistente = _context.Pacientes.FirstOrDefault(p => p.PacienteID == pacienteDTO.pacienteID);
                if (pacienteExistente == null)
                {
                    return NotFound("Paciente no encontrado.");
                }

                // Actualizar los campos del paciente
                pacienteExistente.nombre = pacienteDTO.nombre;
                pacienteExistente.apellido = pacienteDTO.apellido;
                pacienteExistente.correo_electronico = pacienteDTO.correo_electronico;
                pacienteExistente.Telefono = pacienteDTO.Telefono;
                pacienteExistente.Cedula = pacienteDTO.Cedula;
                pacienteExistente.Direccion = pacienteDTO.Direccion;
                pacienteExistente.Fecha_Nacimiento = pacienteDTO.Fecha_Nacimiento;

                // Guardar los cambios en la base de datos
                _context.SaveChanges();

                // Retornar una respuesta de éxito
                return Ok(new { message = "Paciente actualizado exitosamente", pacienteID = pacienteExistente.PacienteID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al actualizar el paciente.", details = ex.Message });
            }
        }
    }
}
