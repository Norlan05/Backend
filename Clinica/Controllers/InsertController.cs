using Clinica_Dr_Toruño.data;
using Microsoft.AspNetCore.Mvc;
using Clinica_Dr_Toruño.Models;
using Clinica_Dr_Toruño.Model_request;
using System.Globalization;
using Clinica_Dr_Toruño.Models.Clinica_Dr_Toruño.Models;

namespace Clinica_Dr_Toruño.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsertController : ControllerBase
    {
        private readonly ClinicaDbcontext db;

        public InsertController(ClinicaDbcontext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Create(reservaDTO model)
        {
            // Validar si el modelo es válido
            if (model == null)
            {
                return BadRequest("El modelo no puede ser nulo.");
            }

            // Validar si los campos requeridos están presentes
            if (string.IsNullOrEmpty(model.nombre) ||
                string.IsNullOrEmpty(model.correo_electronico) ||
                string.IsNullOrEmpty(model.hora) ||
                model.fecha == null)
            {
                return BadRequest("Faltan datos obligatorios. Asegúrese de proporcionar nombre, correo electrónico, hora y fecha.");
            }

            try
            {
                string horaString = model.hora;
                string formatoHora = "h:mm tt"; // Formato para AM/PM

                // Parsear la hora
                DateTime fechaHora = DateTime.ParseExact(horaString, formatoHora, CultureInfo.InvariantCulture);
                TimeSpan hora = fechaHora.TimeOfDay; // Extraer solo la hora

                // Crear el objeto 'reservas'
                var reserva = new reservas()
                {
                    nombre = model.nombre,
                    apellido = model.apellido,
                    correo_electronico = model.correo_electronico,
                    numero_telefono = model.numero_telefono,
                    fecha = model.fecha.Date, // Asegurarse de usar solo la fecha
                    hora = hora, // Usar solo la parte de la hora
                    fecha_hora = model.fecha.Date + hora // Combinar fecha y hora
                };

                // Guardar en la base de datos
                db.Reservas.Add(reserva);
                db.SaveChanges();

                // Retornar la reserva creada
                return Ok(reserva);
            }
            catch (FormatException ex)
            {
                return BadRequest($"Error al procesar la fecha o la hora: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }
    }
}
