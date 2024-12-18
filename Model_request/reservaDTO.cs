namespace CLINICA.Model_request
{
    public class reservaDTO
    {
        public string nombre { get; set; }
        public string? apellido { get; set; }
        public string correo_electronico { get; set; }
        public string? numero_telefono { get; set; }
        public DateTime fecha { get; set; }
        public string? hora { get; set; }
        public string Estado { get; set; } = "Pendiente";  // Estado predeterminado "Pendiente"
        public string Cedula { get; set; }  // Cédula del paciente
    }
}
