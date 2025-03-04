// Controllers/InsertPacienteController.cs
using CLINICA.Data;
using CLINICA.Modelos;
using CLINICA.Model_request;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroConsultaController : ControllerBase
    {
        private readonly ClinicaDbcontext _db;

        public RegistroConsultaController(ClinicaDbcontext db)
        {
            _db = db;
        }

        // Acción POST para registrar un nuevo paciente
        [HttpPost]
        public IActionResult InsertarPaciente(PacienteDTO model)
        {
            // Validación de campos obligatorios
            if (string.IsNullOrEmpty(model.nombre) || string.IsNullOrEmpty(model.Cedula))
            {
                return BadRequest(new { error = "El nombre y la cédula son obligatorios." });
            }

            // Verificar si ya existe un paciente con la misma cédula o correo electrónico
            var pacienteExistente = _db.Pacientes
                .FirstOrDefault(p => p.Cedula == model.Cedula || p.correo_electronico == model.correo_electronico);
            if (pacienteExistente != null)
            {
                return Conflict(new { error = "Ya existe un paciente con esta cédula o correo electrónico." });
            }

            // Crear un nuevo objeto paciente a partir del modelo recibido
            var nuevoPaciente = new Pacientes
            {
                nombre = model.nombre,
                apellido = model.apellido,
                correo_electronico = model.correo_electronico,
                Telefono = model.Telefono,
                Cedula = model.Cedula,
                Direccion = model.Direccion,
                Fecha_Nacimiento = model.Fecha_Nacimiento
            };

            // Insertar el nuevo paciente en la base de datos
            _db.Pacientes.Add(nuevoPaciente);
            _db.SaveChanges();

            // Retornar el paciente recién creado
            return Ok(nuevoPaciente);

        }

        [HttpPost("insert")]
        public IActionResult InsertConsulta(insertDTO model)
        {
            try
            {
                var reserva = _db.Reservas.Find(model.ReservaID);
                if (reserva == null)
                {
                    return NotFound("No se encontró una reserva con el ID proporcionado.");
                }

                var insert = new insert()
                {
                    ReservaID = model.ReservaID,
                    Motivo_Consulta = model.Motivo_Consulta,
                    Diagnostico = model.Diagnostico,
                    Observaciones = model.Observaciones
                };

                _db.Consultas.Add(insert);
                _db.SaveChanges();

                // Crear el DTO con la respuesta deseada
                var response = new insertDTO
                {
                    ReservaID = insert.ReservaID,
                    Motivo_Consulta = insert.Motivo_Consulta,
                    Diagnostico = insert.Diagnostico,
                    Observaciones = insert.Observaciones
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al guardar la consulta.", details = ex.Message });
            }
        }

    }
}