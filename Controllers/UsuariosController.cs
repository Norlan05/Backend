using Microsoft.AspNetCore.Mvc;
using CLINICA.Data;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ClinicaDbcontext _context;

        public UsuariosController(ClinicaDbcontext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.usuarios
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    u.Password
                })
                .ToList();

            return Ok(usuarios);
        }
    }
}
