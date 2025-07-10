using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservasApp.MVC.Models;

namespace ReservasApp.MVC.Data
{
    /// <summary>
    /// Contexto de base de datos principal
    /// ¿Por qué hereda de IdentityDbContext? Porque incluye automáticamente:
    /// - Tablas de usuarios, roles, claims, logins
    /// - Configuraciones de seguridad predefinidas
    /// - Integración completa con ASP.NET Identity
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para nuestras entidades personalizadas
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<QRLink> QRLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de la entidad Reserva
            builder.Entity<Reserva>(entity =>
            {
                // Índices para mejorar rendimiento
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
                      .OnDelete(DeleteBehavior.Cascade); // Si se elimina usuario, se eliminan sus reservas
            });

            // Configuración de la entidad QRLink
            builder.Entity<QRLink>(entity =>
            {
                // Índices para búsquedas rápidas
                entity.HasIndex(q => q.Hash)
                      .IsUnique()
                      .HasDatabaseName("IX_QRLinks_Hash");
                
                entity.HasIndex(q => q.FechaExpiracion)
                      .HasDatabaseName("IX_QRLinks_FechaExpiracion");

                // Configuración de la relación con Reserva (opcional)
                entity.HasOne(q => q.Reserva)
                      .WithMany()
                      .HasForeignKey(q => q.ReservaId)
                      .OnDelete(DeleteBehavior.SetNull); // Si se elimina reserva, el QR queda sin relación
            });

            // Datos de prueba (seed data)
            SeedData(builder);
        }

        /// <summary>
        /// Método para insertar datos de prueba
        /// ¿Por qué seed data? Para tener datos iniciales y poder probar la aplicación inmediatamente
        /// </summary>
        private void SeedData(ModelBuilder builder)
        {
            // No agregamos usuarios aquí porque ASP.NET Identity los maneja de forma especial
            // Los datos de prueba los crearemos en un servicio separado
        }
    }
}
