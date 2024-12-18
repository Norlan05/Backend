namespace CLINICA.Model_request
{
    public class PacienteDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo_Electronico { get; set; }
        public string Telefono { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
    }
}
