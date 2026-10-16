using InfarlabSis.Domain.Entities;
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

        // ==========================================
        // Módulo de Ventas y Devoluciones
        // ==========================================
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();
        public DbSet<Devolucion> Devoluciones => Set<Devolucion>();
        public DbSet<DetalleDevolucion> DetalleDevoluciones => Set<DetalleDevolucion>();

        // ==========================================
        // Módulo de Compras
        // ==========================================
        public DbSet<CompraProveedor> CompraProveedores => Set<CompraProveedor>();
        public DbSet<VentaHistorial> VentaHistorials => Set<VentaHistorial>();
        public DbSet<DetalleVentaHistorial> DetalleVentaHistorials => Set<DetalleVentaHistorial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------------------
            // 1. Mapeo: Seguridad y Actores
            // ------------------------------------------
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.IdPersona);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ApPat).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ApMat).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CI).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.IdRol);
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.NombreUsuario).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Contrasena).HasMaxLength(255).IsRequired();

                entity.HasOne(u => u.Persona)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey<Usuario>(u => u.IdPersona)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.IdRefreshToken);
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();

                entity.HasOne(rt => rt.Usuario)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);

                entity.HasOne(c => c.Persona)
                      .WithMany()
                      .HasForeignKey(c => c.IdPersona)
                      .OnDelete(DeleteBehavior.Restrict);
            });

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
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(250);
            });

            modelBuilder.Entity<Almacen>(entity =>
            {
                entity.HasKey(e => e.IdAlmacen);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(250);
                entity.Property(e => e.TipoArea).HasMaxLength(50).IsRequired();
            });

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

            // ------------------------------------------
            // 3. Mapeo: Ventas y Devoluciones
            // ------------------------------------------
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.IdVenta);

                entity.HasOne(v => v.Usuario)
                      .WithMany()
                      .HasForeignKey(v => v.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Cliente)
                      .WithMany()
                      .HasForeignKey(v => v.IdCliente)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasKey(e => e.IdDetalleVenta);

                entity.HasOne(d => d.Venta)
                      .WithMany(v => v.DetallesVenta)
                      .HasForeignKey(d => d.IdVenta)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Producto)
                      .WithMany()
                      .HasForeignKey(d => d.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Devolucion>(entity =>
            {
                entity.HasKey(e => e.IdDevolucion);

                entity.HasOne(d => d.Venta)
                      .WithMany(v => v.Devoluciones)
                      .HasForeignKey(d => d.IdVenta)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Cliente)
                      .WithMany()
                      .HasForeignKey(d => d.IdCliente)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetalleDevolucion>(entity =>
            {
                entity.HasKey(e => e.IdDetalleDevolucion);

                entity.HasOne(d => d.Devolucion)
                      .WithMany(dev => dev.DetallesDevolucion)
                      .HasForeignKey(d => d.IdDevolucion)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Producto)
                      .WithMany()
                      .HasForeignKey(d => d.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ------------------------------------------
            // 4. Mapeo: Compras
            // ------------------------------------------
            modelBuilder.Entity<CompraProveedor>(entity =>
            {
                entity.HasKey(e => e.IdCompra);

                entity.HasOne(c => c.Proveedor)
                      .WithMany()
                      .HasForeignKey(c => c.IdProveedor)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Categoria)
                      .WithMany()
                      .HasForeignKey(c => c.IdCategoria)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //VENTAHISTORIAL
            modelBuilder.Entity<VentaHistorial>(entity =>
            {
                entity.HasKey(e => e.IdHistorial);

                entity.HasOne(e => e.UsuarioModifico)
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuarioModifico)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.TipoCambio).HasMaxLength(50);
                entity.Property(e => e.DatosAntes).HasColumnType("nvarchar(max)");
                entity.Property(e => e.DatosDespues).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Observaciones).HasMaxLength(500);
            });

            //DETALLEVENTAHISTORIAL
            modelBuilder.Entity<DetalleVentaHistorial>(entity =>
            {
                entity.HasKey(e => e.IdDetalleHistorial);
                entity.Property(e => e.NombreProducto).HasMaxLength(200);
                entity.Property(e => e.Accion).HasMaxLength(50);
            });
        }
    }
}