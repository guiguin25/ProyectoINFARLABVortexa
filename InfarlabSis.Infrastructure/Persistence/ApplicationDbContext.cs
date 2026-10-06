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

        // ==========================================
        // Módulo de Usuarios, Seguridad y Actores
        // ==========================================
        public DbSet<Persona> Personas => Set<Persona>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();

        // ==========================================
        // Módulo de Productos, Almacenes y Calidad
        // ==========================================
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Almacen> Almacenes => Set<Almacen>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Lote> Lotes => Set<Lote>();
        public DbSet<AreaCalidad> AreasCalidad => Set<AreaCalidad>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------------------
            // 1. Mapeo: Seguridad y Actores
            // ------------------------------------------

            // Tabla Persona
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.IdPersona);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ApPat).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ApMat).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CI).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            });

            // Tabla Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.IdRol);
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
            });

            // Tabla Usuario
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

            // Tabla RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.IdRefreshToken);
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();

                entity.HasOne(rt => rt.Usuario)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Tabla Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);

                entity.HasOne(c => c.Persona)
                      .WithMany()
                      .HasForeignKey(c => c.IdPersona)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Tabla Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.IdProveedor);
                entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();

                entity.HasOne(pr => pr.Persona)
                      .WithMany()
                      .HasForeignKey(pr => pr.IdPersona)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------
            // 2. Mapeo: Productos, Almacenes y Calidad
            // ------------------------------------------

            // Tabla Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(250);
            });

            // Tabla Almacen
            modelBuilder.Entity<Almacen>(entity =>
            {
                entity.HasKey(e => e.IdAlmacen);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(250);
                entity.Property(e => e.TipoArea).HasMaxLength(50).IsRequired();
            });

            // Tabla Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.IdProducto);
                entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.PrecioVenta).HasPrecision(10, 2);

                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.IdCategoria)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Proveedor)
                      .WithMany(pr => pr.Productos)
                      .HasForeignKey(p => p.IdProveedor)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Tabla Lote
            modelBuilder.Entity<Lote>(entity =>
            {
                entity.HasKey(e => e.IdLote);
                entity.Property(e => e.Tipo).HasMaxLength(50).IsRequired();

                entity.HasOne(l => l.Producto)
                      .WithMany(p => p.Lotes)
                      .HasForeignKey(l => l.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Proveedor)
                      .WithMany(pr => pr.Lotes)
                      .HasForeignKey(l => l.IdProveedor)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Tabla AreaCalidad (Control de Calidad)
            modelBuilder.Entity<AreaCalidad>(entity =>
            {
                entity.HasKey(e => e.IdArea);
                entity.Property(e => e.Descripcion).HasMaxLength(500);

                entity.HasOne(ac => ac.Lote)
                      .WithMany(l => l.EvaluacionesCalidad)
                      .HasForeignKey(ac => ac.IdLote)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ac => ac.AlmacenDestino)
                      .WithMany()
                      .HasForeignKey(ac => ac.IdAlmacenDestino)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
