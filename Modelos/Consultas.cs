namespace CLINICA.Modelos
{
    public class Consulta
    {
        public int ConsultaID { get; set; } // Este será generado automáticamente
        public int ReservaID { get; set; }
        public int PacienteID { get; set; }
        public required string Motivo_Consulta { get; set; }
        public required string Diagnostico { get; set; }
        public required string Observaciones { get; set; }
        public DateTime Fecha_Consulta { get; set; }

        // Propiedades de navegación
        // Propiedad de navegación para la relación con 'Reserva'
        public virtual reservas Reserva { get; set; }  // Relación con la entidad 'reservas'

        public virtual Pacientes Paciente { get; set; }  // Relación con la entidad 'Pacientes'
    }
}
