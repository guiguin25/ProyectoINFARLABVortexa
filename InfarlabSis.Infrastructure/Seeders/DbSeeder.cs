using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfarlabSis.Infrastructure.Seeders
{
    public static class DbSeeder
    {
        private const string AdminEmail = "admin@infarlab.com";
        private const string AdminRoleName = "Administrador";

        private static readonly (string Name, string Description)[] Roles =
        {
            ("Administrador", "Administrador del sistema"),
            ("Encargado de Logistica", "Gestión de logística"),
            ("Control de Calidad", "Supervisión de calidad"),
            ("Almacen Materia Prima", "Gestión de materia prima"),
            ("Almacen Producto Terminado", "Gestión de producto terminado"),
            ("Ventas", "Área de ventas")
        };

        private static readonly (string Name, string Description, string Type)[] Warehouses =
        {
            ("Almacén Central", "Almacén principal", "Almacenamiento"),
            ("Cuarentena / Inspección", "Productos pendientes de inspección", "Cuarentena"),
            ("Almacén Producto Terminado", "Productos terminados aptos para despacho", "Producto Terminado"),
            ("Rechazados", "Productos rechazados y separados", "Rechazados")
        };

        private static readonly (string Name, string Description)[] Categories =
        {
            ("Materia Prima", "Materias primas"),
            ("Insumos Médicos", "Insumos y materiales médicos"),
            ("Producto Terminado", "Productos terminados")
        };

        private static readonly string[] Units = { "Unidad", "Caja", "Rollo", "Metro" };

        public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            foreach (var roleSeed in Roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Nombre == roleSeed.Name, cancellationToken))
                {
                    context.Roles.Add(new Rol
                    {
                        Nombre = roleSeed.Name,
                        Descripcion = roleSeed.Description,
                        Estado = true
                    });
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            var adminRole = await context.Roles
                .SingleAsync(r => r.Nombre == AdminRoleName, cancellationToken);

            if (!await context.Usuarios.AnyAsync(u => u.Email == AdminEmail, cancellationToken))
            {
                var personaAdmin = await context.Personas
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.Email == AdminEmail, cancellationToken);

                if (personaAdmin is null)
                {
                    personaAdmin = new Persona
                    {
                        ApPat = "Administrador",
                        ApMat = string.Empty,
                        Nombre = "Sistema",
                        CI = "ADMIN-INFARLAB",
                        Direccion = "",
                        Telefono = "",
                        Email = AdminEmail,
                        Estado = true
                    };
                    context.Personas.Add(personaAdmin);
                }
                else if (personaAdmin.Usuario is not null)
                {
                    throw new InvalidOperationException("La persona admin@infarlab.com ya está asociada a otro usuario.");
                }

                personaAdmin.Usuario = new Usuario
                {
                    NombreUsuario = "admin",
                    Email = AdminEmail,
                    Contrasena = BCrypt.Net.BCrypt.HashPassword("123456"),
                    IdRol = adminRole.IdRol,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow
                };
            }

            foreach (var warehouseSeed in Warehouses)
            {
                if (!await context.Almacenes.AnyAsync(a => a.Nombre == warehouseSeed.Name, cancellationToken))
                {
                    context.Almacenes.Add(new Almacen
                    {
                        Nombre = warehouseSeed.Name,
                        Descripcion = warehouseSeed.Description,
                        TipoArea = warehouseSeed.Type,
                        Estado = true
                    });
                }
            }

            foreach (var categorySeed in Categories)
            {
                if (!await context.Categorias.AnyAsync(c => c.Nombre == categorySeed.Name, cancellationToken))
                {
                    context.Categorias.Add(new Categoria
                    {
                        Nombre = categorySeed.Name,
                        Descripcion = categorySeed.Description,
                        Estado = true
                    });
                }
            }

            foreach (var unitName in Units)
            {
                if (!await context.UnidadesMedida.AnyAsync(u => u.Nombre == unitName, cancellationToken))
                {
                    context.UnidadesMedida.Add(new UnidadMedida { Nombre = unitName, Estado = true });
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
