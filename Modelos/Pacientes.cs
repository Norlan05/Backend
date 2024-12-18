using System.ComponentModel.DataAnnotations;

namespace CLINICA.Modelos
{
    public class Pacientes
    {
        [Key]
        public int PacienteID { get; set; }   // Identificador único
        public string Nombre { get; set; }     // Nombre del paciente
        public string Apellido { get; set; }   // Apellido del paciente
        public string Correo_Electronico { get; set; } // Correo electrónico
        public string Telefono { get; set; }   // Número de teléfono
        public string Cedula { get; set; }     // Cédula del paciente
        public string Direccion { get; set; }  // Dirección del paciente
        public DateTime Fecha_Nacimiento { get; set; } // Fecha de nacimiento
    }
}
