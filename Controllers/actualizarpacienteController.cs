using CLINICA.Data;
using CLINICA.Model_request;
using CLINICA.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;

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

                // Buscar al usuario por el correo viejo
                var usuarioExistente = _context.usuarios
                    .FirstOrDefault(u => u.Email == pacienteExistente.correo_electronico);

                if (usuarioExistente == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Actualizar los datos del paciente
                pacienteExistente.nombre = pacienteDTO.nombre;
                pacienteExistente.apellido = pacienteDTO.apellido;
                pacienteExistente.correo_electronico = pacienteDTO.correo_electronico;
                pacienteExistente.Telefono = pacienteDTO.Telefono;
                pacienteExistente.Cedula = pacienteDTO.Cedula;
                pacienteExistente.Direccion = pacienteDTO.Direccion;
                pacienteExistente.Fecha_Nacimiento = pacienteDTO.Fecha_Nacimiento;

                // Actualizar el correo del usuario en la tabla de Usuarios
                if (usuarioExistente.Email != pacienteDTO.correo_electronico)
                {
                    usuarioExistente.Email = pacienteDTO.correo_electronico;
                }

                // Guardar los cambios en la base de datos
                _context.SaveChanges();

                // Enviar el correo al nuevo correo electrónico
                string subject = "Notificación de cambio de correo electrónico";
                string body = $@"
                        <!DOCTYPE html>
                        <html lang='es'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Notificación de Cambio de Correo</title>
                            <style>
                                body {{
                                    font-family: 'Arial', sans-serif;
                                    background-color: #f4f4f4;
                                    margin: 0;
                                    padding: 0;
                                    color: #333;
                                }}
                                .email-container {{
                                    width: 600px;
                                    margin: 20px auto;
                                    padding: 20px;
                                    background-color: #ffffff;
                                    border-radius: 8px;
                                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                }}
                                h1 {{
                                    color: #4CAF50;
                                    font-size: 24px;
                                    text-align: center;
                                    margin-bottom: 20px;
                                }}
                                h2 {{
                                    color: #333;
                                    font-size: 20px;
                                    margin-bottom: 10px;
                                }}
                                .user-details {{
                                    background-color: #f1f1f1;
                                    padding: 15px;
                                    border-radius: 8px;
                                    margin-bottom: 20px;
                                }}
                                .highlight {{
                                    font-weight: bold;
                                    color: #4CAF50;
                                }}
                                p {{
                                    line-height: 1.6;
                                    margin-bottom: 15px;
                                }}
                                .footer {{
                                    font-size: 12px;
                                    color: #888;
                                    text-align: center;
                                    margin-top: 20px;
                                }}
                                .footer p {{
                                    margin: 5px 0;
                                }}
                                .footer a {{
                                    color: #4CAF50;
                                    text-decoration: none;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='email-container'>
                                <h1>🎉 ¡Hola, {pacienteDTO.nombre} {pacienteDTO.apellido}!</h1>
                                <p>Te informamos que tu correo electrónico ha sido actualizado correctamente en nuestro sistema. A continuación, te dejamos tus detalles actualizados:</p>

                                <div class='user-details'>
                                    <h2>📋 Detalles de tu cuenta:</h2>
                                    <p>👤 <strong>Usuario:</strong> <span class='highlight'>{usuarioExistente.Username}</span></p>
                                    <p>🔐 <strong>Contraseña:</strong> <span class='highlight'>{usuarioExistente.Password}</span></p>
                                    <p>📧 <strong>Correo Electrónico:</strong> {usuarioExistente.Email}</p>
                                </div>

                                <p><strong>Para acceder a tu cuenta, utiliza los siguientes datos en nuestro sistema:</strong></p>

                                <div class='user-details'>
                                    <p>👤 <strong>Username:</strong> <span class='highlight'>{usuarioExistente.Username}</span></p>
                                    <p>🔐 <strong>Password:</strong> <span class='highlight'>{usuarioExistente.Password}</span></p>
                                </div>

                                <div class='footer'>
                                    <p><strong>📌 Clínica Dr. Toruño</strong></p>
                                    <p>📍 Dirección: Del INSS dos cuadras al sur</p>
                                    <p>📞 Teléfono de contacto: +505 86288420</p>
                                    <p>📧 Correo electrónico: <a href='mailto:somoto1975@gmail.com'>somoto1975@gmail.com</a></p>
                                    <p>⚕️ Gracias por elegirnos para tu cuidado. ¡Nos vemos pronto!</p>
                                </div>
                            </div>
                        </body>
                        </html>";


                bool emailEnviado = Enviaremail(pacienteDTO.correo_electronico, subject, body);

                if (!emailEnviado)
                {
                    return StatusCode(500, new { error = "Error al enviar el correo." });
                }

                // Retornar respuesta de éxito
                return Ok(new { message = "Paciente y correo actualizado exitosamente.", pacienteID = pacienteExistente.PacienteID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al actualizar el paciente.", details = ex.Message });
            }
        }

        // Método para enviar correo (puedes incluir tu código de envío aquí)
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
    }
}
