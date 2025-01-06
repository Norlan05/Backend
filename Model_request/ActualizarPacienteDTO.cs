namespace CLINICA.Model_request
{
    public class ActualizarPacienteDTO
    {

        public int pacienteID { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string correo_electronico { get; set; }
        public string Telefono { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
    }
}
