using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservasApp.WebAPI.Models;

namespace ReservasApp.WebAPI.Data
{
    /// <summary>
    /// Contexto de base de datos para la API
    /// ¿Por qué reutilizar estructura? Para compartir la misma BD con el MVC
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para nuestras entidades
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<QRLink> QRLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración idéntica al MVC para mantener compatibilidad
            ConfigurarReserva(builder);
            ConfigurarQRLink(builder);
        }

        private void ConfigurarReserva(ModelBuilder builder)
        {
            builder.Entity<Reserva>(entity =>
            {
                // Índices para mejorar rendimiento en la API
                entity.HasIndex(r => r.FechaReserva)
                      .HasDatabaseName("IX_Reservas_FechaReserva");
                
                entity.HasIndex(r => r.UsuarioId)
                      .HasDatabaseName("IX_Reservas_UsuarioId");
                
                entity.HasIndex(r => r.Estado)
                      .HasDatabaseName("IX_Reservas_Estado");

                // Configuración de la relación con Usuario
                entity.HasOne(r => r.Usuario)
                      .WithMany(u => u.Reservas)
                      .HasForeignKey(r => r.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurarQRLink(ModelBuilder builder)
        {
            builder.Entity<QRLink>(entity =>
            {
                // Índices para búsquedas rápidas en la API
                entity.HasIndex(q => q.Hash)
                      .IsUnique()
                      .HasDatabaseName("IX_QRLinks_Hash");
                
                entity.HasIndex(q => q.FechaExpiracion)
                      .HasDatabaseName("IX_QRLinks_FechaExpiracion");

                // Configuración de la relación con Reserva
                entity.HasOne(q => q.Reserva)
                      .WithMany()
                      .HasForeignKey(q => q.ReservaId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
