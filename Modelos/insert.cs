namespace CLINICA.Modelos
{
    public class insert
    {
       
        public int ReservaID { get; set; }
        public int ConsultaID { get; set; }
        public required string Motivo_Consulta { get; set; }
        public required string Diagnostico { get; set; }
        public required string Observaciones { get; set; }
       
    }
}
