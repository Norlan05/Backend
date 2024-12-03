using CLINICA.Model_request;
using Microsoft.AspNetCore.Mvc;
using CLINICA.Modelos;
using CLINICA.Data;

namespace CLINICA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SelectController : ControllerBase
    {
        //ClinicaDbcontext db;

        //public SelectController(ClinicaDbcontext db)
        //{
        //    this.db = db;
        //}


        [HttpPost]
        public IActionResult Index(Cliente_search model)
        {

           
            //coneccion base de datos

            // var cliente_hoy = db.clientes.where(c => c.email == model.email).select(c => c).firstOrderDefaul();

            //


            //db.add(cliente)
            //db.savechange()


            //

            clientes cliente_encontrado = new clientes
            {
          //      id_cliente = cliente_hoy.id_cliente,
                nombre = "C la come",
                apellido = "doblada",
            };  
            return Ok(cliente_encontrado);
        }
    }
}
