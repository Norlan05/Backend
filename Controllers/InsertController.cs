using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using CLINICA.Data;
using CLINICA.Model_request;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;

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

                // Validación de que no se haya hecho una reserva para esta fecha por la misma cédula o correo
                var reservaPorCedulaOCorreo = db.Reservas
                    .FirstOrDefault(r => (r.Cedula == model.Cedula || r.correo_electronico == model.correo_electronico) && r.fecha.Date == model.fecha.Date);

                if (reservaPorCedulaOCorreo != null)
                {
                    return Conflict("Ya tienes una reserva para esta fecha. Solo se permite una reserva por día.");
                }

                // Si no hay conflicto, crear la nueva reserva
                model_ = new reservas()
                {
                    nombre = model.nombre,
                    apellido = model.apellido,
                    correo_electronico = model.correo_electronico,
                    numero_telefono = model.numero_telefono,
                    Cedula = model.Cedula, // Cédula del paciente
                    fecha = model.fecha.Date,
                    hora = hora,
                    fecha_hora = model.fecha.Date + hora, // Combina la fecha y hora
                    estado_id = 1, // Default estado_id
                    estado_descripcion = "Pendiente" // Default estado_descripcion
                };

                db.Reservas.Add(model_);
                db.SaveChanges();

                // Plantilla HTML para el correo detallado
                string subject = "Confirmación de Reserva";
                string body = $@"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Confirmación de Reserva</title>
                        <style>
                            body {{ font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; }}
                            .email-container {{ width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1); }}
                            h1 {{ color: #4CAF50; text-align: center; }}
                            p {{ font-size: 16px; line-height: 1.6; }}
                            .footer {{ font-size: 12px; color: #888; text-align: center; margin-top: 20px; }}
                            .details {{ background-color: #f9f9f9; padding: 10px; margin-top: 20px; border-radius: 5px; }}
                            .details p {{ margin: 5px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <h1>Confirmación de tu reserva</h1>
                            <p>Estimado/a <strong>{model.nombre} {model.apellido},</strong></p>
                            <p>Gracias por reservar con nosotros. A continuación, te enviamos los detalles de tu cita:</p>

                            <div class='details'>
                                <p><strong>Nombre:</strong> {model.nombre} {model.apellido}</p>
                                <p><strong>Correo Electrónico:</strong> {model.correo_electronico}</p>
                                <p><strong>Teléfono:</strong> {model.numero_telefono}</p>
                                <p><strong>Cédula:</strong> {model.Cedula}</p>
                                <p><strong>Fecha de la cita:</strong> {model.fecha.ToString("dd/MM/yyyy")}</p>
                                <p><strong>Hora de la cita:</strong> {model.hora}</p>
                            </div>

                            <p>Por favor, asegúrate de llegar 10 minutos antes de tu cita. En caso de necesitar reprogramar o cancelar, contáctanos con antelación.</p>

                            <div class='footer'>
                                <p>Este es un mensaje automático generado por nuestro sistema. No es necesario que respondas este correo.</p>
                                <p>Si tienes alguna pregunta, no dudes en ponerte en contacto con nosotros.</p>
                                <p><strong>Clínica Dr. Toruño</strong></p>
                            </div>
                        </div>
                    </body>
                    </html>";

                Enviaremail(model.correo_electronico, subject, body);

                return Ok(model_);
            }
            else
            {
                return BadRequest("La hora proporcionada no puede ser nula o vacía.");
            }
        }

        // Función para enviar el correo
        public static bool Enviaremail(string correo, string asunto, string mensaje)
        {
            try
            {
                MailMessage mail = new()
                {
                    To = { new MailAddress(correo) },
                    From = new MailAddress("citasclinicadrtoruno@gmail.com"),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };
                SmtpClient smpt = new()
                {
                    Credentials = new NetworkCredential("citasclinicadrtoruno@gmail.com", "ysdawylalosstpmt"),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true
                };
                smpt.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("CheckReservation")]
        public IActionResult CheckReservation(string cedula, string email, DateTime date)
        {
            var reservaExistente = db.Reservas
                .FirstOrDefault(r => (r.Cedula == cedula || r.correo_electronico == email) && r.fecha.Date == date.Date);

            if (reservaExistente != null)
            {
                return Ok(new { exists = true });
            }

            return Ok(new { exists = false });
        }
    }
}
