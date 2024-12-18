using CLINICA.Modelos;
using Microsoft.EntityFrameworkCore;
namespace CLINICA.Data
{
    public partial class ClinicaDbcontext: DbContext
    {
        public ClinicaDbcontext() 
        {
        }
        public ClinicaDbcontext(DbContextOptions<ClinicaDbcontext> options)
            : base(options)
        {
        }
        public virtual DbSet<reservas> Reservas { get; set; }
        public virtual DbSet<Pacientes> Pacientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<reservas>(entity => 
            {
                entity.ToTable("reservas","dbo");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.nombre).HasColumnName("nombre");
                entity.Property(e => e.apellido).HasColumnName("apellido");
                entity.Property(e => e.correo_electronico).HasColumnName("correo_electronico");
                entity.Property(e => e.numero_telefono).HasColumnName("numero_telefono");

                entity.Property(e => e.fecha).HasColumnName("fecha");
                entity.Property(e => e.hora).HasColumnName("hora");

                entity.Property(e => e.fecha_hora).HasColumnName("fecha_hora");
                // Nuevos campos
                entity.Property(e => e.Estado).HasColumnName("Estado").HasDefaultValue("Pendiente");  // Estado predeterminado
                entity.Property(e => e.Cedula).HasColumnName("Cedula");
            });

            // Configuración para la entidad "Pacientes"
            modelBuilder.Entity<Pacientes>(entity =>
            {
                entity.HasKey(e => e.PacienteID);  // Definir la clave primaria

                entity.ToTable("Pacientes","dbo");

                entity.Property(e => e.PacienteID).HasColumnName("PacienteID");
                entity.Property(e => e.Nombre).HasColumnName("Nombre");
                entity.Property(e => e.Apellido).HasColumnName("Apellido");
                entity.Property(e => e.Correo_Electronico).HasColumnName("Correo_Electronico");
                entity.Property(e => e.Telefono).HasColumnName("Telefono");
                entity.Property(e => e.Cedula).HasColumnName("Cedula");
                entity.Property(e => e.Direccion).HasColumnName("Direccion");
                entity.Property(e => e.Fecha_Nacimiento).HasColumnName("Fecha_Nacimiento");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
