using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using CLINICA.Data;
using CLINICA.Model_request;
using System.Globalization;
using System.Linq;

namespace CLINICA.Controllers
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
        public IActionResult Index(reservaDTO model)
        {
            string horaString = model.hora;
            reservas model_ = new reservas();

            // Definir el formato de hora esperado
            string formatoHora = "h:mm tt"; // "h:mm tt" para "1:00 PM"

            TimeSpan hora;

            if (!string.IsNullOrEmpty(horaString))
            {
                try
                {
                    DateTime fechaHora = DateTime.ParseExact(horaString, formatoHora, CultureInfo.InvariantCulture);
                    hora = fechaHora.TimeOfDay;
                }
                catch (FormatException)
                {
                    return BadRequest("La hora proporcionada no tiene un formato válido. Use 'h:mm AM/PM'.");
                }

                // Validación de existencia de reserva
                var reservaExistente = db.Reservas
                    .FirstOrDefault(r => r.fecha.Date == model.fecha.Date && r.hora == hora);

                if (reservaExistente != null)
                {
                    return Conflict("Ya existe una reserva para esta fecha y hora. Elige otra.");
                }

                // Si no hay conflicto, crear la nueva reserva
                model_ = new reservas()
                {
                    nombre = model.nombre,
                    apellido = model.apellido,
                    correo_electronico = model.correo_electronico,
                    numero_telefono = model.numero_telefono,
                    fecha = model.fecha.Date,
                    hora = hora,
                    fecha_hora = model.fecha.Date + hora // Combina la fecha y hora
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
