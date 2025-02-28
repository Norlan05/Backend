namespace CLINICA.Model_request
{
    public class ActualizarEstadoDTO
    {
        public string id { get; set; }  // ID de la reserva
        public int estado_id { get; set; }  // Estado de la reserva
        public string nombre { get; set; }  // Nombre del paciente
        public string apellido { get; set; }  // Apellido del paciente
        public string correo_electronico { get; set; }  // Correo electrónico del paciente
        public string numero_telefono { get; set; }  // Número de teléfono del paciente
        public string Cedula { get; set; }  // Cédula del paciente
        public DateTime fecha { get; set; }  // Fecha de la cita
        public string hora { get; set; }  // Hora de la cita
    }
}
