namespace Clinica_Dr_Toruño.Model_request
{
    public class reservaDTO
    {
        public string nombre { get; set; }
        public string? apellido { get; set; }
        public string correo_electronico { get; set; }
        public string? numero_telefono { get; set; }
        public DateTime fecha { get; set; }  // Recibe solo la fecha
        public string hora { get; set; }  // Recibe la hora en formato "h:mm AM/PM"
    }
}
