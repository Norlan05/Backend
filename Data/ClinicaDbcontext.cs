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
               
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
