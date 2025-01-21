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
            var (error_in_requet, validate) = ValidateRequest(model);
            if (validate) return BadRequest(error_in_requet);

            // Intentar encontrar un usuario con el correo electrónico y la contraseña proporcionados
            var user = _context.usuarios
                .Where(c => c.Email == model.Email && c.Password == model.Password)
                .FirstOrDefault();

            // Verificar si el usuario o la contraseña son NULL o inválidos
            if (user != null && !string.IsNullOrEmpty(user.Password))
            {
                // Si el login es exitoso, devolver "Acceso correcto"
                return Ok(new { message = "Acceso correcto" });
            }
            else
            {
                // Si las credenciales no son correctas, entonces verificar si hay un token de restablecimiento de contraseña
                var user_reset = _context.usuarios
                    .Where(c => c.Email == model.Email && c.ResetTokenExpiry.HasValue && c.ResetTokenExpiry.Value.AddMinutes(5) >= DateTime.Now)
                    .FirstOrDefault();

                if (user_reset != null)
                {
                    // Si el token de restablecimiento es válido, restablecer la contraseña
                    user_reset.Password = user_reset.ResetToken; // Usar el token como la nueva contraseña
                    _context.usuarios.Update(user_reset);
                    await _context.SaveChangesAsync();

                    // Devuelve el mensaje de "Contraseña restablecida con éxito"
                    return Ok(new { message = "Contraseña restablecida con éxito" });
                }

                // Si no es posible hacer login ni restablecer la contraseña, devolver error
                return BadRequest(new { message = "Valide las credenciales nuevamente" });
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
        // POST: api/Auth/reset
        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> ResetPassword(ResetTokenDTO model)
        {
            // Validar el modelo de la solicitud
            var (error_in_request, validate) = ValidateRequest(model);
            if (validate) return BadRequest(error_in_request);

            // Buscar el usuario que coincida con el email y el reset token
            var user_reset = _context.usuarios
                .Where(c => c.Email == model.Email && c.ResetToken == model.ResetToken && c.ResetTokenExpiry.HasValue)
                .FirstOrDefault();

            if (user_reset != null)
            {
                // Obtener la fecha de expiración del token
                var expiryTime = user_reset.ResetTokenExpiry.Value;
                var currentTime = DateTime.Now;

                // Calcular la diferencia entre la fecha de expiración y la fecha actual
                var timeDifference = expiryTime - currentTime;

                // Verificar si han pasado más de 5 minutos
                if (timeDifference.TotalMinutes <= 0)  // Si el tiempo restante es 0 o negativo, el token ha expirado
                {
                    return BadRequest(new { message = "El token ha expirado." });
                }

                // Si el token no ha expirado, restablecer la contraseña con la nueva proporcionada
                user_reset.Password = model.NewPassword;  // Usar la nueva contraseña proporcionada por el usuario
                user_reset.ResetToken = null;  // Invalidar el token después de usarlo
                user_reset.ResetTokenExpiry = null;  // Eliminar la fecha de expiración del token

                _context.usuarios.Update(user_reset);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Contraseña restablecida con éxito" });
            }

            // Si el token no es válido o no se encuentra el usuario, devolver error
            return BadRequest(new { message = "Token de restablecimiento inválido o no encontrado" });
        }

        // Método para validar los datos de la solicitud entrante
        [ApiExplorerSettings(IgnoreApi = true)]
        public (ErrorModel, bool) ValidateRequest(ResetTokenDTO model)
        {
            ErrorModel error = new ErrorModel();

            // Verificar si el campo email está vacío
            if (string.IsNullOrEmpty(model.Email))
            {
                error.status_error = "Campo email es necesario";
                error.error_text = "Campo requerido";
                return (error, true);
            }
            // Verificar si el campo reset token está vacío
            else if (string.IsNullOrEmpty(model.ResetToken))
            {
                error.status_error = "Campo ResetToken es necesario";
                error.error_text = "Campo requerido";
                return (error, true);
            }
            // Verificar si el campo nueva contraseña está vacío
            else if (string.IsNullOrEmpty(model.NewPassword))
            {
                error.status_error = "Campo nueva contraseña es necesario";
                error.error_text = "Campo requerido";
                return (error, true);
            }

            return (error, false);
        }


    }
}


