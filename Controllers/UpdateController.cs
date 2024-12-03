using Microsoft.AspNetCore.Mvc;

namespace CLINICA.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : ControllerBase
    {
        [HttpPost]
        public IActionResult Index()
        {
            return Ok("Listo");
        }
    }
}
