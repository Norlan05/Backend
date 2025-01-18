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
        public virtual DbSet<insert> Consultas { get; set; } // Asegúrate de que la clase Consultas esté definida en el espacio de nombres CLINICA.Modelos
        public virtual DbSet<Pacientes> Pacientes { get; set; }
        public virtual DbSet<estados> Estados { get; set; } // Asegúrate de agregar el DbSet para Estado
        public virtual DbSet<Usuario> usuarios { get; set; }  // Tabla Users  


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
            modelBuilder.Entity<insert>(entity =>
            {
                entity.HasKey(e => e.ConsultaID);
                entity.ToTable("Consultas", "dbo");

                entity.Property(e => e.ConsultaID).HasColumnName("ConsultaID");
                entity.Property(e => e.ReservaID).HasColumnName("ReservaID");
                entity.Property(e => e.Motivo_Consulta).HasColumnName("Motivo_Consulta").IsRequired();
                entity.Property(e => e.Diagnostico).HasColumnName("Diagnostico").IsRequired();
                entity.Property(e => e.Observaciones).HasColumnName("Observaciones");

                entity.HasOne<reservas>()
                    .WithMany()
                    .HasForeignKey(e => e.ReservaID)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.ToTable("Usuarios", "dbo");
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.Username).HasColumnName("username");
                entity.Property(c => c.Email).HasColumnName("Email");
                entity.Property(c => c.Password).HasColumnName("Password");
                entity.Property(c => c.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(c => c.ResetToken).HasColumnName("ResetToken");
                entity.Property(c => c.ResetTokenExpiry).HasColumnName("ResetTokenExpiry");  // Aquí corrige el nombre
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
