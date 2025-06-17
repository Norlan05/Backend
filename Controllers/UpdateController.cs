using CLINICA.Data;
using CLINICA.Model_request;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly ClinicaDbcontext _db;

        public UpdateController(ClinicaDbcontext db)
        {
            _db = db;
        }

        [HttpPost("Actualizar")]
        public IActionResult ActualizarEstadoReserva([FromBody] ActualizarEstadoDTO model)
        {
            if (string.IsNullOrEmpty(model.id))
            {
                return BadRequest("El id de la reserva es requerido.");
            }
            if (model.estado_id < 1 || model.estado_id > 3)
            {
                return BadRequest("El estado_id debe ser 1 (Pendiente), 2 (Confirmada) o 3 (Cancelada).");
            }

            var reserva = _db.Reservas.FirstOrDefault(r => r.id == int.Parse(model.id));

            if (reserva == null)
            {
                return NotFound("Reserva no encontrada.");
            }

            var estado = _db.Estados.FirstOrDefault(e => e.estado_id == model.estado_id);

            if (estado == null)
            {
                return NotFound("Estado no encontrado.");
            }

            // ✅ Solo actualizamos el estado, no eliminamos
            reserva.estado_id = estado.estado_id;
            reserva.estado_descripcion = estado.descripcion;
            _db.SaveChanges();

            if (model.estado_id == 3)
            {

                string subject = "Cancelación de cita";
                string body = $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Cancelación de Cita</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; margin: 0; padding: 0; }}
                        .email-container {{ width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1); }}
                        h1 {{ color: #F44336; text-align: center; font-size: 24px; margin-bottom: 20px; }}
                        p {{ font-size: 16px; line-height: 1.6; margin: 0; }}
                        .footer {{ font-size: 12px; color: #888; text-align: center; margin-top: 20px; }}
                        .details {{ background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 5px; }}
                        .details p {{ margin: 5px 0; }}
                        .important {{ color: #FF5722; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <h1>❌ Cancelación de Cita</h1>
                        <p>Estimado/a <strong>{model.nombre} {model.apellido},</strong></p>
                        <p>Lamentamos informarte que tu cita programada ha sido cancelada. A continuación, te compartimos los detalles de la cita cancelada:</p>

                        <h2>📅 Detalles de la cita cancelada</h2>
                        <div class='details'>
                            <p>👤 <strong>Nombre:</strong> {model.nombre} {model.apellido}</p>
                            <p>📧 <strong>Correo Electrónico:</strong> {model.correo_electronico}</p>
                            <p>📞 <strong>Teléfono:</strong> {model.numero_telefono}</p>
                            <p>🆔 <strong>Cédula:</strong> {model.Cedula}</p>
                            <p>📆 <strong>Fecha solicitada:</strong> {model.fecha.ToString("dd/MM/yyyy")}</p>
                            <p>🕒 <strong>Hora solicitada:</strong> {model.hora}</p>
                        </div>

                        <p><strong class='important'>Si deseas reagendar tu cita o necesitas asistencia adicional, no dudes en ponerte en contacto con nosotros.</strong></p>
                        <p>Nos disculpamos por cualquier inconveniente que esto pueda haberte ocasionado y estamos aquí para ayudarte en todo lo que necesites.</p>

                        <div class='footer'>
                            <p>⚕️ Gracias por elegirnos para tu cuidado. ¡Esperamos verte pronto!</p>
                            <p>📝 Este es un mensaje automático, por favor no respondas a este correo.</p>
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

                return Ok(new { mensaje = "Cita cancelada y hora liberada correctamente." });
            }

           

            string subjectConfirmacion = "Confirmación de cita";
            string bodyConfirmacion = $@"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Confirmación de Cita</title>
                <style>
                    body {{ font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; margin: 0; padding: 0; }}
                    .email-container {{ width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border-radius: 8px; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1); }}
                    h1 {{ color: #4CAF50; text-align: center; font-size: 24px; margin-bottom: 20px; }}
                    p {{ font-size: 16px; line-height: 1.6; margin: 0; }}
                    .footer {{ font-size: 12px; color: #888; text-align: center; margin-top: 20px; }}
                    .details {{ background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 5px; }}
                    .details p {{ margin: 5px 0; }}
                    .important {{ color: #FF5722; font-weight: bold; }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>📩 Confirmación de Cita</h1>
                    <p>Estimado/a <strong>{model.nombre} {model.apellido},</strong></p>
                    <p>Nos complace informarte que tu cita ha sido confirmada. A continuación, te compartimos los detalles de tu cita:</p>

                    <h2>📅 Detalles de tu cita confirmada</h2>
                    <div class='details'>
                        <p>👤 <strong>Nombre:</strong> {model.nombre} {model.apellido}</p>
                        <p>📧 <strong>Correo Electrónico:</strong> {model.correo_electronico}</p>
                        <p>📞 <strong>Teléfono:</strong> {model.numero_telefono}</p>
                        <p>🆔 <strong>Cédula:</strong> {model.Cedula}</p>
                        <p>📆 <strong>Fecha de la cita:</strong> {model.fecha.ToString("dd/MM/yyyy")}</p>
                        <p>🕒 <strong>Hora de la cita:</strong> {model.hora}</p>
                    </div>

                    <p><strong class='important'>¡Te esperamos en nuestra clínica en la fecha y hora programadas!</strong></p>
                    <p>Si necesitas realizar alguna modificación o tienes alguna pregunta, no dudes en contactarnos a través de nuestro correo electrónico o llamándonos directamente.</p>

                    <div class='footer'>
                        <p>⚕️ Gracias por elegirnos para tu cuidado. ¡Nos vemos pronto!</p>
                        <p>📝 Este es un mensaje automático, por favor no respondas a este correo.</p>
                    </div>
                </div>
            </body>
            </html>";

            bool emailConfirmado = Enviaremail(model.correo_electronico, subjectConfirmacion, bodyConfirmacion);

            if (emailConfirmado)
            {
                Console.WriteLine("Correo enviado exitosamente a " + model.correo_electronico);
            }
            else
            {
                Console.WriteLine("Fallo al enviar el correo a " + model.correo_electronico);
            }

            return Ok(new { mensaje = "Estado de la reserva actualizado correctamente." });
        }

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

                SmtpClient smtp = new()
                {
                    Credentials = new NetworkCredential("citasclinicadrtoruno@gmail.com", "ysdawylalosstpmt"),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true
                };

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
