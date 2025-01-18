using CLINICA.Data;
using CLINICA.Modelos;
using CLINICA.Model_request;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using static CLINICA.Controllers.AuthController;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResetController : ControllerBase
    {
        private readonly ClinicaDbcontext db;

        public ResetController(ClinicaDbcontext db)
        {
            this.db = db;
        }

        [HttpPost]
        [Route("password")]
        public IActionResult Update_password(ResetDto model)
        {
            GeneradorNumerosAleatorios generate = new();
            var user = db.usuarios.FirstOrDefault(c => c.Email == model.email);

            if (user == null)
            {
                return BadRequest(new ErrorModel
                {
                    error_text = "Valide las credenciales nuevamente",
                    status_error = "Error en reset"
                });
            }

            user.ResetToken = generate.generar_pass();
            user.ResetTokenExpiry = DateTime.Now.AddMinutes(30);  // Tiempo de expiración del token

            db.usuarios.Update(user);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                return StatusCode(500, new { error = "Error al actualizar el usuario", message = ex.Message });
            }

            // Plantilla HTML con el formato solicitado
            string htmlTemplate = @"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Correo Estético</title>
                <style>
                    body { font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; }
                    .email-container { width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; }
                    h1 { color: #4CAF50; }
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>Estimado {{Username}},</h1>
                    <p>Gracias por confiar en nuestros servicios. A continuación, te proporcionamos la información solicitada:</p>
                    <p>Tu contraseña temporal es: <strong>{0}</strong></p>
                    <p>Por razones de seguridad, te recomendamos cambiarla tan pronto como accedas a tu cuenta.</p>
                    <div class='footer'>
                        <p>Este es un mensaje automático generado por nuestro sistema. Por favor, no respondas a este correo.</p>
                        <p>Si no has solicitado un restablecimiento de contraseña, por favor contacta con nuestro soporte a la brevedad.</p>
                    </div>
                </div>
            </body>
            </html>";

            // Reemplaza {{Username}} con el nombre de usuario y {0} con la contraseña temporal
            string emailContent = htmlTemplate.Replace("{{Username}}", user.Username)
                                              .Replace("{0}", user.ResetToken);

            // Enviar correo
            Enviaremail(model.email, "Restablecimiento de contraseña", emailContent);

            return Ok(new { message = "Token de reinicio enviado exitosamente" });
        }

        public class GeneradorNumerosAleatorios
        {
            public string generar_pass()
            {
                Random random = new();
                string password = "";
                for (int i = 0; i < 5; i++)
                {
                    password += random.Next(0, 10).ToString();
                }
                return password;
            }
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
    }
}
