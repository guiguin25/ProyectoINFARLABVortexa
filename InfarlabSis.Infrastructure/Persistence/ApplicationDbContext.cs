using InfarlabSis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace InfarlabSis.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Persona> Personas => Set<Persona>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo Tabla Persona
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.IdPersona);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ApPat).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ApMat).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CI).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            });

            // Mapeo Tabla Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.IdRol);
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
            });

            // Mapeo Tabla Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.NombreUsuario).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Contrasena).HasMaxLength(255).IsRequired();

                // Relación 1:1 con Persona
                entity.HasOne(u => u.Persona)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey<Usuario>(u => u.IdPersona)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación 1:N con Rol
                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Mapeo Tabla RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.IdRefreshToken);
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();

                entity.HasOne(rt => rt.Usuario)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
