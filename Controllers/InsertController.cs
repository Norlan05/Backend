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
            Console.WriteLine("Iniciando proceso de reserva...");
            string horaString = model.hora;
            reservas model_ = new reservas();

            string formatoHora = "h:mm tt";
            TimeSpan hora;

            if (!string.IsNullOrEmpty(horaString))
            {
                try
                {
                    DateTime fechaHora = DateTime.ParseExact(horaString, formatoHora, CultureInfo.InvariantCulture);
                    hora = fechaHora.TimeOfDay;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Error en el formato de la hora: " + ex.Message);
                    return BadRequest("La hora proporcionada no tiene un formato válido. Use 'h:mm AM/PM'.");
                }

                var reservaExistente = db.Reservas.FirstOrDefault(r => r.fecha.Date == model.fecha.Date && r.hora == hora);
                if (reservaExistente != null)
                {
                    Console.WriteLine("Reserva ya existente para esta fecha y hora.");
                    return Conflict("Ya existe una reserva para esta fecha y hora. Elige otra.");
                }

                var reservaPorCedulaOCorreo = db.Reservas.FirstOrDefault(r => (r.Cedula == model.Cedula || r.correo_electronico == model.correo_electronico) && r.fecha.Date == model.fecha.Date);
                if (reservaPorCedulaOCorreo != null)
                {
                    Console.WriteLine("El usuario ya tiene una reserva para esta fecha.");
                    return Conflict("Ya tienes una reserva para esta fecha. Solo se permite una reserva por día.");
                }

                model_ = new reservas()
                {
                    nombre = model.nombre,
                    apellido = model.apellido,
                    correo_electronico = model.correo_electronico,
                    numero_telefono = model.numero_telefono,
                    Cedula = model.Cedula,
                    fecha = model.fecha.Date,
                    hora = hora,
                    fecha_hora = model.fecha.Date + hora,
                    estado_id = 1,
                    estado_descripcion = "Pendiente"
                };

                db.Reservas.Add(model_);
                db.SaveChanges();
                Console.WriteLine("Reserva guardada correctamente en la base de datos.");
                // Plantilla HTML para el correo de recepción de solicitud de cita
                string subject = "Recepción de solicitud de cita";
                string body = $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Recepción de Solicitud de Cita</title>
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
                        <h1>📩 Recepción de solicitud de cita</h1>
                        <p>Estimado/a <strong>{model.nombre} {model.apellido},</strong></p>
                        <p>Hemos recibido tu solicitud de cita desde nuestra página web y queremos agradecerte por confiar en nosotros para tu atención médica. Nos pondremos en contacto contigo en breve para confirmar la disponibilidad y asegurarnos de que todos los detalles de tu cita sean correctos.</p>

                        <h2>📅 Detalles de la cita solicitada</h2>
                        <div class='details'>
                            <p>👤 <strong>Nombre:</strong> {model.nombre} {model.apellido}</p>
                            <p>📧 <strong>Correo Electrónico:</strong> {model.correo_electronico}</p>
                            <p>📞 <strong>Teléfono:</strong> {model.numero_telefono}</p>
                            <p>🆔 <strong>Cédula:</strong> {model.Cedula}</p>
                            <p>📆 <strong>Fecha solicitada:</strong> {model.fecha.ToString("dd/MM/yyyy")}</p>
                            <p>🕒 <strong>Hora solicitada:</strong> {model.hora}</p>
                        </div>

                        <p><strong>⚠️ Importante:</strong> Esta solicitud aún no ha sido confirmada. Nos pondremos en contacto contigo lo antes posible para verificar la disponibilidad de la fecha y hora seleccionadas. En caso de necesitar modificar o cancelar tu solicitud, puedes escribirnos a nuestro correo o llamarnos directamente.</p>

                        <p>Si tienes alguna pregunta o necesitas asistencia, no dudes en comunicarte con nuestro equipo. Estaremos encantados de ayudarte.</p>

                        <div class='footer'>
                            <p><strong>📌 Clínica Dr. Toruño</strong></p>
                            <p>📍 Dirección: Del INSS dos cuadras al sur</p>
                            <p>📞 Teléfono de contacto: +505 86288420</p>
                            <p>📧 Correo electrónico: somoto1975@gmail.com</p>
                            <p>⚕️ Gracias por elegirnos para tu cuidado. ¡Nos vemos pronto!</p>
                            <p>📝 Este es un mensaje automático. No es necesario que respondas este correo.</p>
                        </div>
                    </div>
                </body>
                </html>";
                bool emailEnviado = Enviaremail(model.correo_electronico, subject, body);

                if (emailEnviado)
                {
                    Console.WriteLine("Correo enviado exitosamente a " + model.correo_electronico);
                }
                else
                {
                    Console.WriteLine("Fallo al enviar el correo a " + model.correo_electronico);
                }

                return Ok(model_);
            }
            else
            {
                Console.WriteLine("La hora proporcionada es nula o vacía.");
                return BadRequest("La hora proporcionada no puede ser nula o vacía.");
            }
        }

        public static bool Enviaremail(string correo, string asunto, string mensaje)
        {
            try
            {
                Console.WriteLine("Iniciando el envío de correo...");
                MailMessage mail = new()
                {
                    To = { new MailAddress(correo) },
                    From = new MailAddress("citasclinicadrtoruno@gmail.com"),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };

                SmtpClient smtp = new()
                {
                    Credentials = new NetworkCredential("citasclinicadrtoruno@gmail.com", "ysdawylalosstpmt"),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true
                };

                smtp.Send(mail);
                Console.WriteLine("Correo enviado exitosamente a " + correo);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo: " + ex.Message);
                return false;
            }
        }

        [HttpGet("CheckReservation")]
        public IActionResult CheckReservation(string cedula, string email, DateTime date)
        {
            var reservaExistente = db.Reservas.FirstOrDefault(r => (r.Cedula == cedula || r.correo_electronico == email) && r.fecha.Date == date.Date);
            if (reservaExistente != null)
            {
                return Ok(new { exists = true });
            }
            return Ok(new { exists = false });
        }
    }
}
