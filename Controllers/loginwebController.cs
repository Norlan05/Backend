using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CLINICA.Modelos;
using CLINICA.Model_request;
using CLINICA.Data;
using System.Linq;

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

        // POST: api/LoginWeb/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginWithUsername([FromBody] LoginWithUsernameRequestDTO model)
        {
            // Validar los datos de la solicitud
            var (error_in_request, validate) = ValidateRequest(model);
            if (validate) return BadRequest(error_in_request);

            // Intentar encontrar un usuario con el username y la contraseña proporcionados
            var user = _context.usuarios
                .Where(c => c.Username == model.Username && c.Password == model.Password)
                .FirstOrDefault();

            // Verificar si el usuario o la contraseña son NULL o inválidos
            if (user != null && !string.IsNullOrEmpty(user.Password))
            {
                // Si el login es exitoso, devolver "Acceso correcto"
                return Ok(new { message = "Acceso correcto" });
            }

            // Si las credenciales no son correctas, devolver error
            return BadRequest(new { message = "Usuario o contraseña incorrectos" });
        }

        // Método para validar los datos de la solicitud entrante
        [ApiExplorerSettings(IgnoreApi = true)]
        public (ErrorModel, bool) ValidateRequest(LoginWithUsernameRequestDTO model)
        {
            ErrorModel error = new ErrorModel();

            if (string.IsNullOrEmpty(model.Username)) // Validar por 'Username'
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

        // Definición de ErrorModel
        public class ErrorModel
        {
            public string status_error { get; set; }
            public string error_text { get; set; }
        }
    }

}
