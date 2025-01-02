using System.ComponentModel.DataAnnotations;

namespace CLINICA.Modelos
{
    public class estados
    {
        [Key] // Indica que esta propiedad es la clave primaria
        public int estado_id { get; set; }  // ID del estado
        public string descripcion { get; set; }  // Descripción del estado (Pendiente, Confirmada, etc.)

        // Propiedad de navegación para la relación inversa
        public ICollection<reservas> Reservas { get; set; }  // Estado puede tener muchas reservas
    }
}
