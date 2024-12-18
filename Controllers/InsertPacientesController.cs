using CLINICA.Data;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public PacientesController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pacientes>>> GetPacientes()
        {
            return await _context.Pacientes.ToListAsync();
        }

        // GET: api/Pacientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pacientes>> GetPaciente(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);

            if (paciente == null)
            {
                return NotFound();
            }

            return paciente;
        }

        // POST: api/Pacientes
        [HttpPost]
        public async Task<ActionResult<Pacientes>> PostPaciente(Pacientes paciente)
        {
            // Validación adicional
            if (string.IsNullOrEmpty(paciente.Nombre) || string.IsNullOrEmpty(paciente.Cedula))
            {
                return BadRequest("El nombre y la Cedula son obligatorios.");
            }

            // Verificar si ya existe un paciente con el mismo teléfono o correo
            var pacienteExistente = await _context.Pacientes
                                                  .FirstOrDefaultAsync(p => p.Cedula == paciente.Telefono || p.Correo_Electronico == paciente.Correo_Electronico);
            if (pacienteExistente != null)
            {
                return Conflict("Ya existe un paciente con esa Cedula o correo.");
            }

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaciente", new { id = paciente.PacienteID }, paciente);
        }

        // PUT: api/Pacientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(int id, Pacientes paciente)
        {
            if (id != paciente.PacienteID)
            {
                return BadRequest();
            }

            _context.Entry(paciente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Pacientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
