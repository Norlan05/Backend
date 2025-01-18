using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CLINICA.Modelos;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CLINICA.Model_request;
using CLINICA.Data;

namespace CLINICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public AuthController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (await _context.usuarios.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "El nombre de usuario ya está en uso" });
            }

            if (await _context.usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "El correo electrónico ya está en uso" });
            }

            var usuario = new Usuario
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,  // Texto plano
                CreatedAt = DateTime.Now
            };

            _context.usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Usuario registrado exitosamente" });
        }


        // POST: api/Auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> get_user(LoginRequestDTO model)
        {
            // Validar el modelo de la solicitud
            (var error_in_requet, bool validate) = ValidateRequest(model);
            if (validate) return BadRequest(error_in_requet);

            // Verificar si el usuario tiene un token de reinicio y si está dentro del tiempo de expiración
            var user_reset = _context.usuarios
                .Where(c => c.Email == model.Email && c.ResetTokenExpiry.HasValue && c.ResetTokenExpiry.Value.AddMinutes(5) >= DateTime.Now)
                .FirstOrDefault();

            if (user_reset != null)
            {
                // Restablecer la contraseña si el token de reinicio es válido
                user_reset.Password = user_reset.ResetToken;
                _context.usuarios.Update(user_reset);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Contraseña restablecida con éxito. Acceso correcto." });
            }
            else
            {
                // Intentar encontrar un usuario con el correo electrónico y la contraseña proporcionados
                var user = _context.usuarios
                    .Where(c => c.Email == model.Email && c.Password == model.Password)
                    .Select(a => new { a.Email, a.Password }) // Selecciona solo los campos necesarios
                    .FirstOrDefault();

                // Verificar si el usuario o la contraseña son NULL o inválidos
                if (user == null || string.IsNullOrEmpty(user.Password))
                {
                    var error = new
                    {
                        error_text = "Valide las credenciales nuevamente",
                        status_error = "Error en login"
                    };
                    return BadRequest(error);
                }

                // Retornar Ok si el usuario es encontrado y la contraseña es correcta
                return Ok(new { message = "Acceso correcto" });
            }
        }

        // Método para validar los datos de la solicitud entrante
        [ApiExplorerSettings(IgnoreApi = true)]
        public (ErrorModel, bool) ValidateRequest(LoginRequestDTO model)
        {
            ErrorModel error = new ErrorModel();

            if (string.IsNullOrEmpty(model.Email))
            {
                error.status_error = "Campo email es necesario";
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

        // Definición de ErrorModel
        public class ErrorModel
        {
            public string status_error { get; set; }
            public string error_text { get; set; }

        }
    }
}

