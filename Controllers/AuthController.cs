using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CLINICA.Modelos;
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
            if (await _context.Usuarios.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "El nombre de usuario ya está en uso" });
            }

            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
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

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Usuario registrado exitosamente" });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // Usar FirstOrDefaultAsync para obtener el usuario directamente y solo traer la contraseña
            var user = await _context.Usuarios
                .Where(u => u.Username == request.Username)
                .Select(u => new { u.Password, u.Email })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado" });
            }

            // Comparar la contraseña en texto plano
            if (user.Password != request.Password)
            {
                return Unauthorized(new { message = "Contraseña incorrecta" });
            }

            return Ok(new { message = "Usuario autenticado con éxito", Email = user.Email });
        }

    }
}
