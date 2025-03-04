
using CLINICA.Data;
using CLINICA.Modelos;
using CLINICA.Model_request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsertarPacienteController : ControllerBase
    {
        private readonly ClinicaDbcontext _db;
        private readonly ClinicaDbcontext _context;  // Asegúrate de tener esta referencia al contexto de usuarios

        public InsertarPacienteController(ClinicaDbcontext db, ClinicaDbcontext context)
        {
            _db = db;
            _context = context;
        }

        // Acción POST para registrar un nuevo paciente y crear un usuario aleatorio
        [HttpPost]
        public async Task<IActionResult> InsertarPaciente(PacienteDTO model)
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
            await _db.SaveChangesAsync();

            // Crear un nombre de usuario aleatorio (por ejemplo, paciente + número aleatorio)
            string nombreUsuario = $"{model.nombre}{model.apellido}{new Random().Next(1000, 9999)}";

            // Crear una contraseña aleatoria de 8 caracteres
            string contrasena = Path.GetRandomFileName().Replace(".", "").Substring(0, 8); // Contraseña aleatoria

            // Crear el usuario correspondiente
            var usuario = new Usuario
            {
                Username = nombreUsuario,
                Email = model.correo_electronico, // Usamos el correo electrónico del paciente
                Password = contrasena,  // Contraseña en texto plano
                CreatedAt = DateTime.Now
            };

            // Insertar el nuevo usuario en la base de datos
            _context.usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Enviar correo electrónico con los datos de usuario y contraseña
            string subject = "Detalles de tu cuenta en nuestra clínica";
            string body = $@"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Bienvenido a la Clínica</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif; 
                                color: #333; 
                                background-color: #f4f4f4; 
                                margin: 0;
                                padding: 0;
                            }}
                            .email-container {{
                                width: 600px; 
                                margin: 20px auto; 
                                padding: 20px; 
                                background-color: #ffffff; 
                                border-radius: 8px; 
                                box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1); 
                                font-size: 16px;
                            }}
                            h1 {{
                                color: #4CAF50; 
                                text-align: center;
                                margin-bottom: 20px;
                            }}
                            h2 {{
                                color: #333;
                                margin-top: 30px;
                                margin-bottom: 10px;
                            }}
                            p {{
                                line-height: 1.6;
                                margin-bottom: 15px;
                            }}
                            .user-details {{
                                background-color: #f9f9f9;
                                padding: 15px;
                                border-radius: 8px;
                                margin-bottom: 30px;
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
                            .highlight {{
                                color: #4CAF50;
                                font-weight: bold;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <h1>🎉 ¡Bienvenido a nuestra clínica, {model.nombre} {model.apellido}!</h1>

                            <p>Hemos creado tu cuenta en nuestro sistema. A continuación, encontrarás tus detalles de acceso:</p>

                            <div class='user-details'>
                                <h2>📋 Detalles de la cuenta:</h2>
                                <p>👤 <strong>Usuario:</strong> <span class='highlight'>{usuario.Username}</span></p>
                                <p>🔐 <strong>Contraseña:</strong> <span class='highlight'>{usuario.Password}</span></p>
                                <p>📧 <strong>Correo Electrónico:</strong> {usuario.Email}</p>
                            </div>

                            <p><strong>Para ingresar a nuestro sistema desde nuestra página web, utiliza solamente los siguientes datos:</strong></p>

                            <div class='user-details'>
                                <p>👤 <strong>Username:</strong> <span class='highlight'>{usuario.Username}</span></p>
                                <p>🔐 <strong>Password:</strong> <span class='highlight'>{usuario.Password}</span></p>
                            </div>

                            <div class='footer'>
                                <p><strong>📌 Clínica Dr. Toruño</strong></p>
                                <p>📍 Dirección: Del INSS dos cuadras al sur</p>
                                <p>📞 Teléfono de contacto: +505 86288420</p>
                                <p>📧 Correo electrónico: somoto1975@gmail.com</p>
                                <p>⚕️ Gracias por elegirnos para tu cuidado. ¡Nos vemos pronto!</p>
                            </div>
                        </div>
                    </body>
                    </html>";


            bool emailEnviado = Enviaremail(model.correo_electronico, subject, body);

            if (!emailEnviado)
            {
                return StatusCode(500, new { error = "Error al enviar el correo." });
            }

            // Retornar el paciente y usuario recién creados
            return Ok(new
            {
                paciente = nuevoPaciente,
                usuario = new
                {
                    usuario.Username,
                    usuario.Email,
                    Password = usuario.Password  // Aquí incluimos la contraseña en texto plano
                }
            });
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
