namespace CLINICA.Modelos
{
    
      public class reservas
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string? apellido { get; set; }
        public string correo_electronico { get; set; }
        public string? numero_telefono { get; set; }
        public DateTime fecha { get; set; }
        public TimeSpan hora { get; set; }  // Esto almacena solo la hora.
        public DateTime fecha_hora { get; set; }  // Combinación de fecha y hora
        public string Estado { get; set; } = "Pendiente";  // Estado de la reserva (Pendiente, Confirmada, Cancelada)
        public string Cedula { get; set; }  // Cédula del paciente
    }
}
