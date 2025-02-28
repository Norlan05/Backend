using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CLINICA.Data;
using CLINICA.Model_request;
using Microsoft.AspNetCore.Http;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginWebController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public LoginWebController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // ✅ Método de login con almacenamiento de sesión
        [HttpPost("login")]
        public IActionResult LoginWithUsername([FromBody] LoginWithUsernameRequestDTO model)
        {
            var (error_in_request, validate) = ValidateRequest(model);
            if (validate) return BadRequest(error_in_request);

            var user = _context.usuarios
                .FirstOrDefault(c => c.Username == model.Username && c.Password == model.Password);

            if (user != null)
            {
                // 🔹 Guardamos el email en la sesión
                HttpContext.Session.SetString("userEmail", user.Email);

                Response.Headers["Access-Control-Allow-Credentials"] = "true";

                return Ok(new
                {
                    message = "Acceso correcto",
                    correo = user.Email
                });
            }

            return BadRequest(new { message = "Usuario o contraseña incorrectos" });
        }

        // ✅ Método para verificar si hay una sesión activa
        [HttpGet("verificarSesion")]
        public IActionResult VerificarSesion()
        {
            string userEmail = HttpContext.Session.GetString("userEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "No hay una sesión activa." });
            }

            return Ok(new { message = "Sesión activa", correo = userEmail });
        }

        // ✅ Validación de los datos del login
        [ApiExplorerSettings(IgnoreApi = true)]
        public (ErrorModel, bool) ValidateRequest(LoginWithUsernameRequestDTO model)
        {
            ErrorModel error = new ErrorModel();

            if (string.IsNullOrEmpty(model.Username))
            {
                error.status_error = "Campo username es necesario";
                error.error_text = "Campo requerido";
                return (error, true);
            }
            else if (string.IsNullOrEmpty(model.Password))
            {
                error.status_error = "Campo password es necesario";
                error.error_text = "Campo requerido";
                return (error, true);
            }

            return (error, false);
        }

        // ✅ Modelo para manejar errores
        public class ErrorModel
        {
            public string status_error { get; set; }
            public string error_text { get; set; }
        }
    }
}
