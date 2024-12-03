using Clinica_Dr_Toruño.Models;
using Clinica_Dr_Toruño.Models.Clinica_Dr_Toruño.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinica_Dr_Toruño.data
{
    public partial class ClinicaDbcontext : DbContext
    {
        public ClinicaDbcontext() { }

        public ClinicaDbcontext(DbContextOptions<ClinicaDbcontext> options)
            : base(options)
        { }

        public virtual DbSet<reservas> Reservas { get; set; }

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
                entity.ToTable("reservas", "dbo");

                // Mapeo de columnas
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.nombre).HasColumnName("nombre");
                entity.Property(e => e.apellido).HasColumnName("apellido");
                entity.Property(e => e.correo_electronico).HasColumnName("correo_electronico");
                entity.Property(e => e.numero_telefono).HasColumnName("numero_telefono");

                // Asegúrate de mapear las propiedades 'fecha' y 'hora'
                entity.Property(e => e.fecha).HasColumnName("fecha");
                entity.Property(e => e.hora).HasColumnName("hora");

                // Mapear 'fecha_hora'
                entity.Property(e => e.fecha_hora).HasColumnName("fecha_hora");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
