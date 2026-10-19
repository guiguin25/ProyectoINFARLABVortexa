using Bogus;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfarlabSis.Infrastructure.Seeders
{
    public static class DbSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            //ROLES
            if (!await context.Roles.AnyAsync())
            {
                await context.Roles.AddRangeAsync(
                    new Rol { Nombre = "Administrador", Descripcion = "Administrador del sistema", Estado = true },
                    new Rol { Nombre = "Encargado de Logistica", Descripcion = "Gestión de logística", Estado = true },
                    new Rol { Nombre = "Control de Calidad", Descripcion = "Supervisión de calidad", Estado = true },
                    new Rol { Nombre = "Almacen Materia Prima", Descripcion = "Gestión de materia prima", Estado = true },
                    new Rol { Nombre = "Almacen Producto Terminado", Descripcion = "Gestión de producto terminado", Estado = true },
                    new Rol { Nombre = "Ventas", Descripcion = "Área de ventas", Estado = true }
                );
                await context.SaveChangesAsync();
            }

            //USUARIO Y PERSONA -- ADMIN
            if (!await context.Personas.AnyAsync(p => p.Email == "admin@infarlab.com"))
            {
                var personaAdmin = new Persona
                {
                    ApPat = "Perez",
                    ApMat = "Gomez",
                    Nombre = "Juan",
                    CI = "1234567",
                    Direccion = "Av. Principal 123",
                    Telefono = "70000000",
                    Email = "admin@infarlab.com",
                    Estado = true,
                    Usuario = new Usuario
                    {
                        NombreUsuario = "admin",
                        Email = "admin@infarlab.com",
                        Contrasena = BCrypt.Net.BCrypt.HashPassword("123456"),
                        IdRol = 1,
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    }
                };
                await context.Personas.AddAsync(personaAdmin);
                await context.SaveChangesAsync();
            }
            //USUARIO Y PERSONA
            if (await context.Personas.CountAsync() < 6) // Si solo está el admin
            {
                var faker = new Faker("es");
                var rolesDisponibles = (await context.Roles.Where(r => r.IdRol != 1).Select(r => r.IdRol).ToListAsync()).ToArray();

                for (int i = 0; i < 10; i++)
                {
                    var persona = new Persona
                    {
                        ApPat = faker.Name.LastName(),
                        ApMat = faker.Name.LastName(),
                        Nombre = faker.Name.FirstName(),
                        CI = faker.Random.Number(1000000, 9999999).ToString(),
                        Direccion = faker.Address.StreetAddress(),
                        Telefono = faker.Phone.PhoneNumber("########"),
                        Email = $"{faker.Internet.UserName()}@infarlab.com",
                        Estado = true,
                        Usuario = new Usuario
                        {
                            NombreUsuario = faker.Internet.UserName(),
                            Email = $"{faker.Internet.UserName()}@infarlab.com",
                            Contrasena = BCrypt.Net.BCrypt.HashPassword("123456"),
                            IdRol = faker.PickRandom(rolesDisponibles),
                            Estado = true,
                            FechaCreacion = faker.Date.Past(1)
                        }
                    };
                    await context.Personas.AddAsync(persona);
                }
                await context.SaveChangesAsync();
            }

            //CATEGORÍAS
            if (!await context.Categorias.AnyAsync())
            {
                await context.Categorias.AddRangeAsync(
                    new Categoria { Nombre = "Batas quirúrgicas desechables", Descripcion = "Batas en talla M", Estado = true },
                    new Categoria { Nombre = "Insumos médicos", Descripcion = "A baterías", Estado = true },
                    new Categoria { Nombre = "Guantes desechables", Descripcion = "Látex", Estado = true },
                    new Categoria { Nombre = "Gorros quirúrgicos desechables", Descripcion = "Equipos y dispositivos", Estado = true },
                    new Categoria { Nombre = "Materiales", Descripcion = "Materiales de curación", Estado = true }
                );
                await context.SaveChangesAsync();
            }

            //PROVEEDORES
            if (!await context.Proveedores.AnyAsync())
            {
                var faker = new Faker("es");

                await context.Proveedores.AddAsync(new Proveedor { Nombre = "Laboratorio Pfizer Bolivia", Documento = null, Direccion = "", Telefono = "", Email = null });
                for (int i = 0; i < 5; i++)
                {
                    await context.Proveedores.AddAsync(new Proveedor
                    {
                        Nombre = faker.Company.CompanyName(),
                        Documento = faker.Random.Replace("#########"),
                        Direccion = faker.Address.FullAddress(),
                        Telefono = faker.Phone.PhoneNumber("########"),
                        Email = faker.Internet.Email(),
                        ContactoNombre = faker.Name.FullName(),
                        ContactoTelefono = faker.Phone.PhoneNumber("########"),
                        ContactoEmail = faker.Internet.Email()
                    });
                }
                await context.SaveChangesAsync();
            }

            //CLIENTES
            if (!await context.Clientes.AnyAsync())
            {
                var faker = new Faker("es");
                var tipos = new[] { "Hospital", "Caja de Salud", "Clínica", "Farmacia", "Laboratorio" };

                for (int i = 0; i < 10; i++)
                {
                    await context.Clientes.AddAsync(new Cliente
                    {
                        RazonSocial = faker.Company.CompanyName(),
                        Nit = faker.Random.Replace("#########"),
                        TipoCliente = faker.PickRandom(tipos),
                        Direccion = faker.Address.FullAddress(),
                        Telefono = faker.Phone.PhoneNumber("########")
                    });
                }
                await context.SaveChangesAsync();
            }

            //ALMACENES
            if (!await context.Almacenes.AnyAsync())
            {
                await context.Almacenes.AddRangeAsync(
                    new Almacen { Nombre = "Almacén Central", Descripcion = "Almacén principal", TipoArea = "Almacenamiento", Estado = true },
                    new Almacen { Nombre = "Farmacia Ventas", Descripcion = "Área de ventas", TipoArea = "Ventas", Estado = true },
                    new Almacen { Nombre = "Cámara Fría", Descripcion = "Almacén refrigerado", TipoArea = "Refrigeración", Estado = true },
                    new Almacen { Nombre = "Materia Prima", Descripcion = "Almacén materia prima", TipoArea = "Producción", Estado = true },
                    new Almacen { Nombre = "Producto Terminado", Descripcion = "Producto terminado", TipoArea = "Producción", Estado = true }
                );
                await context.SaveChangesAsync();
            }

            //PRODUCTOS
            if (!await context.Productos.AnyAsync())
            {
                var faker = new Faker("es");
                var categoriasIds = await context.Categorias.Select(c => c.IdCategoria).ToListAsync();
                var proveedoresIds = await context.Proveedores.Select(p => p.IdProveedor).ToListAsync();

                for (int i = 0; i < 20; i++)
                {
                    await context.Productos.AddAsync(new Producto
                    {
                        IdCategoria = faker.PickRandom(categoriasIds),
                        IdProveedor = faker.PickRandom(proveedoresIds),
                        Nombre = faker.Commerce.ProductName(),
                        Descripcion = faker.Commerce.ProductDescription(),
                        PrecioVenta = Math.Round(faker.Random.Decimal(5, 150), 2),
                        StockActual = faker.Random.Int(0, 500),
                        Estado = true
                    });
                }
                await context.SaveChangesAsync();
            }
            
            //LOTES
            if (!await context.Lotes.AnyAsync())
            {
                var faker = new Faker("es");
                var productosIds = await context.Productos.Select(p => p.IdProducto).ToListAsync();
                var proveedoresIds = await context.Proveedores.Select(p => p.IdProveedor).ToListAsync();

                foreach (var productoId in productosIds)
                {
                    var cantidadLotes = faker.Random.Int(1, 3);
                    for (int i = 0; i < cantidadLotes; i++)
                    {
                        await context.Lotes.AddAsync(new Lote
                        {
                            IdProducto = productoId,
                            IdProveedor = faker.PickRandom(proveedoresIds),
                            Tipo = faker.PickRandom("Compra", "Producción", "Devolución"),
                            FechaElaboracion = faker.Date.Past(2),
                            FechaVencimiento = faker.Date.Future(2)
                        });
                    }
                }
                await context.SaveChangesAsync();
            }

            //COMPRA_PROVEEDOR
            if (!await context.CompraProveedores.AnyAsync())
            {
                var faker = new Faker("es");
                var proveedoresIds = await context.Proveedores.Select(p => p.IdProveedor).ToListAsync();
                var categoriasIds = await context.Categorias.Select(c => c.IdCategoria).ToListAsync();

                for (int i = 0; i < 15; i++)
                {
                    await context.CompraProveedores.AddAsync(new CompraProveedor
                    {
                        IdProveedor = faker.PickRandom(proveedoresIds),
                        IdCategoria = faker.PickRandom(categoriasIds),
                        ProductoComprado = faker.Commerce.ProductName(),
                        FechaCompra = faker.Date.Past(1),
                        Total = Math.Round(faker.Random.Decimal(100, 5000), 2),
                        Estado = true
                    });
                }
                await context.SaveChangesAsync();
            }

            //ÁREA_CALIDAD
            if (!await context.AreasCalidad.AnyAsync())
            {
                var faker = new Faker("es");
                var lotesIds = await context.Lotes.Select(l => l.IdLote).ToListAsync();
                var almacenesIds = await context.Almacenes.Select(a => a.IdAlmacen).ToListAsync();

                var lotesParaEvaluar = faker.PickRandom(lotesIds, Math.Min(10, lotesIds.Count)).ToList();

                foreach (var loteId in lotesParaEvaluar)
                {
                    await context.AreasCalidad.AddAsync(new AreaCalidad
                    {
                        IdLote = loteId,
                        IdAlmacenDestino = faker.PickRandom(almacenesIds),
                        Descripcion = faker.PickRandom("Aprobado", "En cuarentena", "No Aprobado"),
                        FechaEvaluacion = faker.Date.Past(1),
                        FechaResultado = faker.Date.Recent(30)
                    });
                }
                await context.SaveChangesAsync();
            }
            
            //VENTAS
            if (!await context.Ventas.AnyAsync())
            {
                var faker = new Faker("es");
                var usuariosIds = await context.Usuarios.Select(u => u.IdUsuario).ToListAsync();
                var clientesIds = await context.Clientes.Select(c => c.IdCliente).ToListAsync();

                for (int i = 0; i < 10; i++)
                {
                    await context.Ventas.AddAsync(new Venta
                    {
                        IdUsuario = faker.PickRandom(usuariosIds),
                        IdCliente = faker.PickRandom(clientesIds),
                        FechaVenta = faker.Date.Past(1),
                        TipoPago = faker.PickRandom("Efectivo", "Tarjeta"),
                        Total = 0,
                        Estado = true
                    });
                }
                await context.SaveChangesAsync();
            }

            //DETALLE_VENTA
            if (!await context.DetalleVentas.AnyAsync())
            {
                var faker = new Faker("es");
                var ventasIds = await context.Ventas.Select(v => v.IdVenta).ToListAsync();
                var productosIds = await context.Productos.Select(p => p.IdProducto).ToListAsync();

                foreach (var ventaId in ventasIds)
                {
                    var cantidadProductos = faker.Random.Int(2, 5);
                    var productosSeleccionados = faker.PickRandom(productosIds, Math.Min(cantidadProductos, productosIds.Count)).ToList();
                    decimal totalVenta = 0;

                    foreach (var productoId in productosSeleccionados)
                    {
                        var cantidad = faker.Random.Int(1, 5);
                        var precioUnitario = faker.Random.Decimal(10, 100);
                        var subtotal = cantidad * precioUnitario;
                        totalVenta += subtotal;

                        await context.DetalleVentas.AddAsync(new DetalleVenta
                        {
                            IdVenta = ventaId,
                            IdProducto = productoId,
                            Cantidad = cantidad,
                            PrecioUnitario = Math.Round(precioUnitario, 2),
                            Subtotal = Math.Round(subtotal, 2),
                            Estado = true
                        });
                    }

                    var venta = await context.Ventas.FindAsync(ventaId);
                    if (venta != null)
                    {
                        venta.Total = Math.Round(totalVenta, 2);
                        context.Ventas.Update(venta);
                    }
                }
                await context.SaveChangesAsync();
            }
            
            //DEVOLUCIONES
            if (!await context.Devoluciones.AnyAsync())
            {
                var faker = new Faker("es");
                var ventasIds = await context.Ventas.Select(v => v.IdVenta).ToListAsync();
                var clientesIds = await context.Clientes.Select(c => c.IdCliente).ToListAsync();

                for (int i = 0; i < faker.Random.Int(3, 5); i++)
                {
                    await context.Devoluciones.AddAsync(new Devolucion
                    {
                        IdVenta = faker.PickRandom(ventasIds),
                        IdCliente = faker.PickRandom(clientesIds),
                        FechaDevolucion = faker.Date.Recent(30),
                        MotivoDevolucion = faker.PickRandom("Producto defectuoso", "Fecha de vencimiento próxima", "Error en el pedido", "Producto dañado", "Cambio de producto"),
                        Estado = true
                    });
                }
                await context.SaveChangesAsync();
            }

            //DETALLE_DEVOLUCIÓN
            if (!await context.DetalleDevoluciones.AnyAsync())
            {
                var faker = new Faker("es");
                var devolucionesIds = await context.Devoluciones.Select(d => d.IdDevolucion).ToListAsync();
                var productosIds = await context.Productos.Select(p => p.IdProducto).ToListAsync();

                foreach (var devolucionId in devolucionesIds)
                {
                    var cantidadProductos = faker.Random.Int(1, 3);
                    var productosSeleccionados = faker.PickRandom(productosIds, Math.Min(cantidadProductos, productosIds.Count)).ToList();

                    foreach (var productoId in productosSeleccionados)
                    {
                        await context.DetalleDevoluciones.AddAsync(new DetalleDevolucion
                        {
                            IdDevolucion = devolucionId,
                            IdProducto = productoId,
                            Cantidad = faker.Random.Int(1, 3),
                            Estado = true
                        });
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}