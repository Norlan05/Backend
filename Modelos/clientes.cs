namespace CLINICA.Modelos
{
    public class clientes
    {
        public int id_cliente { get; set; }
        public string nombre{ get; set; }
        public string? apellido { get; set; }
        public string? email { get; set; }
        public string? telefono { get; set; }
        public string? direccion { get; set; }
        public string? fecha_nacimiento { get; set; }
        public string? sexo { get; set; }
        public string? created_at { get; set; }
        public string? updated_at { get; set; }
    }
}
