using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using CLINICA.Data;
using CLINICA.Model_request;
using System.Globalization;

namespace CLINICA.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class InsertController : ControllerBase
    {
        ClinicaDbcontext db;

        public InsertController(ClinicaDbcontext db)
        {
            this.db = db;
        }
        [HttpPost]
        public IActionResult Index(reservaDTO model)
        {
            string horaString = model.hora;
            reservas model_ = new reservas();

            // Define el formato esperado de la hora en formato AM/PM
            string formatoHora = "h:mm tt"; // "h:mm tt" para "1:00 PM"

            // Variable para almacenar la hora en formato 24 horas
            TimeSpan hora;

            // Verifica si horaString no es nulo o vacío antes de intentar parsear
            if (!string.IsNullOrEmpty(horaString))
            {
                try
                {
                    // Intenta convertir la hora en formato AM/PM a formato de 24 horas
                    DateTime fechaHora = DateTime.ParseExact(horaString, formatoHora, CultureInfo.InvariantCulture);
                    hora = fechaHora.TimeOfDay;
                }
                catch (FormatException)
                {
                    // Si el formato de la hora es incorrecto, retorna un error
                    return BadRequest("La hora proporcionada no tiene un formato válido. Use 'h:mm AM/PM'.");
                }

                model_ = new reservas()
                {
                    nombre = model.nombre,
                    apellido = model.apellido,
                    correo_electronico = model.correo_electronico,
                    numero_telefono = model.numero_telefono,
                    fecha = model.fecha.Date, // Combina la fecha y la hora
                    hora = hora, // Usar solo la parte de la hora
                    fecha_hora = model.fecha.Date + hora // Combinar fecha y hora

                };

                db.Reservas.Add(model_);
                db.SaveChanges();
                return Ok(model_);
            }
            else
            {
                return BadRequest("La hora proporcionada no puede ser nula o vacía.");
            }
        }
    }
}
