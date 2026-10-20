using System.Text.Json;
using InfarlabSis.Application.DTOs.Ventas;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfarlabApiProject.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Ventas
        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            var ventas = await _context.Ventas
                //.Where(v => v.Estado)
                .Include(v => v.Usuario)
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => new
                {
                    idVenta = v.IdVenta,
                    idCliente = v.IdCliente,
                    idUsuario = v.IdUsuario,
                    nombreCliente = v.Cliente != null ? v.Cliente.RazonSocial : "Cliente Desconocido",
                    nombreUsuario = v.Usuario != null ? v.Usuario.NombreUsuario : "Sistema",
                    fechaVenta = v.FechaVenta,
                    tipoPago = v.TipoPago,
                    total = v.Total,
                    estado = v.Estado ? "Completada" : "Eliminada"
                })
                .ToListAsync();

            return Ok(ventas);
        }

        // ✅ GET: api/Ventas/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Usuario)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
                return NotFound(new { mensaje = "Venta no encontrada." });

            return Ok(new
            {
                idVenta = venta.IdVenta,
                idCliente = venta.IdCliente,
                idUsuario = venta.IdUsuario,
                nombreCliente = venta.Cliente != null ? venta.Cliente.RazonSocial : "Cliente Desconocido",
                fechaVenta = venta.FechaVenta,
                tipoPago = venta.TipoPago,
                total = venta.Total,
                estado = venta.Estado ? "Completada" : "Eliminada",
                detalles = venta.DetallesVenta.Select(d => new
                {
                    idDetalle = d.IdDetalleVenta,
                    idProducto = d.IdProducto,
                    nombreProducto = d.Producto?.Nombre ?? "Producto Desconocido",
                    cantidad = d.Cantidad,
                    precioUnitario = d.PrecioUnitario,
                    subtotal = d.Subtotal
                }).ToList()
            });
        }

        //POST: api/Ventas - Crear venta con historial
        [HttpPost]
        public async Task<IActionResult> CrearVenta([FromBody] VentaCreaDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Console.WriteLine("=== CREAR VENTA - INICIO ===");
                Console.WriteLine($"IdCliente: {dto.IdCliente}, IdUsuario: {dto.IdUsuario}");
                Console.WriteLine($"Detalles: {dto.Detalles.Count} productos");

                // 1. Validar cliente
                var cliente = await _context.Clientes.FindAsync(dto.IdCliente);
                if (cliente == null)
                {
                    Console.WriteLine("ERROR: Cliente no encontrado");
                    return BadRequest(new { mensaje = "El cliente especificado no existe." });
                }

                // 2. Crear la venta
                var nuevaVenta = new Venta
                {
                    FechaVenta = dto.FechaVenta,
                    TipoPago = dto.TipoPago,
                    IdUsuario = dto.IdUsuario,
                    IdCliente = dto.IdCliente,
                    Total = 0,
                    Estado = true,
                    DetallesVenta = new List<DetalleVenta>()
                };

                // 3. Procesar detalles
                decimal totalVenta = 0;
                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalleDto.IdProducto);
                    if (producto == null)
                    {
                        Console.WriteLine($"ERROR: Producto {detalleDto.IdProducto} no encontrado");
                        return BadRequest(new { mensaje = $"Producto ID {detalleDto.IdProducto} no encontrado" });
                    }

                    var subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario;
                    totalVenta += subtotal;

                    nuevaVenta.DetallesVenta.Add(new DetalleVenta
                    {
                        IdProducto = detalleDto.IdProducto,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = detalleDto.PrecioUnitario,
                        Subtotal = subtotal,
                        Estado = true
                    });
                }

                nuevaVenta.Total = totalVenta;

                // 4. Guardar venta
                Console.WriteLine("Guardando venta...");
                _context.Ventas.Add(nuevaVenta);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Venta guardada con ID: {nuevaVenta.IdVenta}");

                // 5. Registrar historial
                try
                {
                    Console.WriteLine("Creando registro de historial...");

                    var datosVenta = new
                    {
                        idVenta = nuevaVenta.IdVenta,
                        idCliente = nuevaVenta.IdCliente,
                        fechaVenta = nuevaVenta.FechaVenta,
                        tipoPago = nuevaVenta.TipoPago,
                        total = nuevaVenta.Total,
                        detalles = nuevaVenta.DetallesVenta.Select(d => new
                        {
                            idProducto = d.IdProducto,
                            cantidad = d.Cantidad,
                            precioUnitario = d.PrecioUnitario,
                            subtotal = d.Subtotal
                        }).ToList()
                    };

                    var historial = new VentaHistorial
                    {
                        IdVenta = nuevaVenta.IdVenta,
                        FechaModificacion = DateTime.UtcNow,
                        IdUsuarioModifico = dto.IdUsuario,
                        TipoCambio = "CREACION",
                        DatosAntes = "{}",
                        DatosDespues = System.Text.Json.JsonSerializer.Serialize(datosVenta),
                        Observaciones = "Venta creada"
                    };

                    Console.WriteLine($"Historial creado: IdVenta={historial.IdVenta}, Tipo={historial.TipoCambio}");
                    Console.WriteLine($"DatosDespues JSON length: {historial.DatosDespues.Length}");

                    _context.VentaHistorials.Add(historial);
                    Console.WriteLine("Guardando historial...");

                    int rowsAffected = await _context.SaveChangesAsync();

                    Console.WriteLine($"Historial guardado. Filas afectadas: {rowsAffected}");
                    Console.WriteLine($"ID Historial generado: {historial.IdHistorial}");
                }
                catch (Exception histEx)
                {
                    Console.WriteLine($"ERROR al guardar historial: {histEx.Message}");
                    Console.WriteLine($"Stack trace: {histEx.StackTrace}");
                    if (histEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {histEx.InnerException.Message}");
                    }
                }

                await transaction.CommitAsync();

                Console.WriteLine("=== CREAR VENTA - FIN ===");
                return Ok(new
                {
                    mensaje = "Venta registrada exitosamente",
                    idVenta = nuevaVenta.IdVenta
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"ERROR CRÍTICO: {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");

                return StatusCode(500, new
                {
                    mensaje = "Error interno: " + ex.Message,
                    detalle = ex.InnerException?.Message
                });
            }
        }

        //PUT: api/Ventas/5 - Actualizar venta con historial
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarVenta(int id, [FromBody] VentaCreaDto dto)
        {
            try
            {
                var ventaExistente = await _context.Ventas
                    .Include(v => v.DetallesVenta)
                    .FirstOrDefaultAsync(v => v.IdVenta == id);

                if (ventaExistente == null)
                    return NotFound(new { mensaje = "Venta no encontrada." });

                //GUARDAR ESTADO DE ANTES
                var datosAntes = new
                {
                    idVenta = ventaExistente.IdVenta,
                    idCliente = ventaExistente.IdCliente,
                    fechaVenta = ventaExistente.FechaVenta,
                    tipoPago = ventaExistente.TipoPago,
                    total = ventaExistente.Total,
                    detalles = ventaExistente.DetallesVenta.Select(d => new
                    {
                        idProducto = d.IdProducto,
                        nombreProducto = d.Producto?.Nombre ?? "Producto",
                        cantidad = d.Cantidad,
                        precioUnitario = d.PrecioUnitario,
                        subtotal = d.Subtotal
                    }).ToList()
                };

                string jsonAntes = System.Text.Json.JsonSerializer.Serialize(datosAntes);
                Console.WriteLine($"[HISTORIAL] Datos ANTES: {jsonAntes}");

                // 2. Validar cliente
                var cliente = await _context.Clientes.FindAsync(dto.IdCliente);
                if (cliente == null)
                    return BadRequest(new { mensaje = "Cliente no existe." });

                // 3. Actualizar datos
                ventaExistente.FechaVenta = dto.FechaVenta;
                ventaExistente.TipoPago = dto.TipoPago;
                ventaExistente.IdCliente = dto.IdCliente;

                // 4. Eliminar detalles antiguos
                _context.DetalleVentas.RemoveRange(ventaExistente.DetallesVenta);
                await _context.SaveChangesAsync();

                // 5. Agregar nuevos detalles
                decimal totalVenta = 0;
                var nuevosDetalles = new List<DetalleVenta>();

                foreach (var detalleDto in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalleDto.IdProducto);
                    if (producto == null)
                        return BadRequest(new { mensaje = $"Producto {detalleDto.IdProducto} no encontrado" });

                    var subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario;
                    totalVenta += subtotal;

                    nuevosDetalles.Add(new DetalleVenta
                    {
                        IdVenta = id,
                        IdProducto = detalleDto.IdProducto,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = detalleDto.PrecioUnitario,
                        Subtotal = subtotal,
                        Estado = true
                    });
                }

                ventaExistente.DetallesVenta = nuevosDetalles;
                ventaExistente.Total = totalVenta;

                //GUARDAR CAMBIOS PRIMERO
                await _context.SaveChangesAsync();
                Console.WriteLine($"[INFO] Venta {id} actualizada y guardada");

                //CREAR Y GUARDAR HISTORIAL DESPUÉS
                try
                {
                    var datosDespues = new
                    {
                        idVenta = ventaExistente.IdVenta,
                        idCliente = ventaExistente.IdCliente,
                        fechaVenta = ventaExistente.FechaVenta,
                        tipoPago = ventaExistente.TipoPago,
                        total = ventaExistente.Total,
                        detalles = ventaExistente.DetallesVenta.Select(d => new
                        {
                            idProducto = d.IdProducto,
                            nombreProducto = d.Producto?.Nombre ?? "Producto",
                            cantidad = d.Cantidad,
                            precioUnitario = d.PrecioUnitario,
                            subtotal = d.Subtotal
                        }).ToList()
                    };

                    string jsonDespues = System.Text.Json.JsonSerializer.Serialize(datosDespues);
                    Console.WriteLine($"[HISTORIAL] Datos DESPUÉS: {jsonDespues}");

                    var historial = new VentaHistorial
                    {
                        IdVenta = id,
                        FechaModificacion = DateTime.UtcNow,
                        IdUsuarioModifico = dto.IdUsuario,
                        TipoCambio = "ACTUALIZACION",
                        DatosAntes = jsonAntes,
                        DatosDespues = jsonDespues,
                        Observaciones = "Venta modificada"
                    };

                    _context.VentaHistorials.Add(historial);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"[INFO] Historial guardado con ID {historial.IdHistorial}");
                }
                catch (Exception histEx)
                {
                    Console.WriteLine($"[ERROR] No se pudo guardar el historial: {histEx.Message}");
                    Console.WriteLine($"[ERROR] Stack trace: {histEx.StackTrace}");
                }

                return Ok(new { mensaje = "Venta actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error al actualizar venta: {ex.Message}");
                Console.WriteLine($"[ERROR] Inner: {ex.InnerException?.Message}");
                return StatusCode(500, new
                {
                    mensaje = "Error: " + ex.Message,
                    detalle = ex.InnerException?.Message
                });
            }
        }

        //DELETE: api/Ventas/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetallesVenta)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
                return NotFound(new { mensaje = "Venta no encontrada." });

            venta.Estado = false;
            foreach (var detalle in venta.DetallesVenta)
            {
                detalle.Estado = false;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Venta eliminada exitosamente." });
        }

        //GET: api/Ventas/5/historial
        [HttpGet("{id:int}/historial")]
        public async Task<IActionResult> GetVentaHistorial(int id)
        {
            try
            {
                var historial = await _context.VentaHistorials
                    .Where(h => h.IdVenta == id)
                    .OrderByDescending(h => h.FechaModificacion)
                    .Select(h => new
                    {
                        idHistorial = h.IdHistorial,
                        fechaModificacion = h.FechaModificacion,
                        idUsuarioModifico = h.IdUsuarioModifico,
                        tipoCambio = h.TipoCambio,
                        datosAntes = h.DatosAntes,
                        datosDespues = h.DatosDespues,
                        observaciones = h.Observaciones
                    })
                    .ToListAsync();

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error: " + ex.Message });
            }
        }
    }
}