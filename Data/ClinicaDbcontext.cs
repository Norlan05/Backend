using CLINICA.Modelos;
using Microsoft.EntityFrameworkCore;

namespace CLINICA.Data
{
    public partial class ClinicaDbcontext : DbContext
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
        public virtual DbSet<estados> Estados { get; set; } // Asegúrate de agregar el DbSet para Estado

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

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.nombre).HasColumnName("nombre");
                entity.Property(e => e.apellido).HasColumnName("apellido");
                entity.Property(e => e.correo_electronico).HasColumnName("correo_electronico");
                entity.Property(e => e.numero_telefono).HasColumnName("numero_telefono");
                entity.Property(e => e.fecha).HasColumnName("fecha");
                entity.Property(e => e.hora).HasColumnName("hora");
                entity.Property(e => e.fecha_hora).HasColumnName("fecha_hora");
                entity.Property(e => e.Cedula).HasColumnName("Cedula");
                entity.Property(e => e.estado_id).HasColumnName("estado_id");

                entity.HasOne<estados>()
                    .WithMany(e => e.Reservas)
                    .HasForeignKey(d => d.estado_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Pacientes>(entity =>
            {
                entity.HasKey(e => e.PacienteID);
                entity.ToTable("Pacientes", "dbo");

                entity.Property(e => e.PacienteID).HasColumnName("PacienteID");
                entity.Property(e => e.nombre).HasColumnName("nombre");
                entity.Property(e => e.apellido).HasColumnName("apellido");
                entity.Property(e => e.correo_electronico).HasColumnName("correo_electronico");
                entity.Property(e => e.Telefono).HasColumnName("Telefono");
                entity.Property(e => e.Cedula).HasColumnName("Cedula");
                entity.Property(e => e.Direccion).HasColumnName("Direccion");
                entity.Property(e => e.Fecha_Nacimiento).HasColumnName("Fecha_Nacimiento");
            });

            modelBuilder.Entity<estados>(entity =>
            {
                entity.HasKey(e => e.estado_id);
                entity.ToTable("estados", "dbo");

                entity.Property(e => e.estado_id).HasColumnName("estado_id");
                entity.Property(e => e.descripcion).HasColumnName("descripcion");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
