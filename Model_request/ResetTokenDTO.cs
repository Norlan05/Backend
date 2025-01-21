namespace CLINICA.Model_request
{
    public class ResetTokenDTO
    {
        public string Email { get; set; }
        public string ResetToken { get; set; }

        public string NewPassword { get; set; }  // Campo para la nueva contraseña
    }
}
