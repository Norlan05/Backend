namespace CLINICA.Model_request
{
    public class HistorialConsultaDTO
    {

        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public string Motivo_Consulta { get; set; }
        public string Diagnostico { get; set; }
        public string Observaciones { get; set; }
        public DateTime Fecha_Reserva { get; set; }
    }
}
