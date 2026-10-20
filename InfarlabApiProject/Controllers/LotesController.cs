using AutoMapper;
using InfarlabSis.Application.DTOs.Productos;
using InfarlabSis.Domain.Entities;
using InfarlabSis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfarlabApiProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LotesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoteDto>>> GetLotes(CancellationToken cancellationToken)
        {
            var lotes = await _context.Lotes.AsNoTracking()
                .Include(l => l.Producto)
                .Include(l => l.Proveedor)
                .OrderBy(l => l.FechaVencimiento)
                .ToListAsync(cancellationToken);
            return Ok(_mapper.Map<List<LoteDto>>(lotes));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoteDto>> GetLote(int id, CancellationToken cancellationToken)
        {
            var lote = await _context.Lotes.AsNoTracking()
                .Include(l => l.Producto)
                .Include(l => l.Proveedor)
                .FirstOrDefaultAsync(l => l.IdLote == id, cancellationToken);
            return lote is null ? NotFound(new { mensaje = "Lote no encontrado." }) : Ok(_mapper.Map<LoteDto>(lote));
        }

        [HttpPost]
        public async Task<ActionResult<LoteDto>> CrearLote([FromBody] CrearLoteDto dto, CancellationToken cancellationToken)
        {
            if (dto.FechaVencimiento < dto.FechaElaboracion)
                return BadRequest(new { mensaje = "La fecha de vencimiento no puede ser anterior a la fecha de elaboración." });

            if (!await _context.Productos.AnyAsync(p => p.IdProducto == dto.IdProducto, cancellationToken))
                return BadRequest(new { mensaje = "El producto indicado no existe." });

            if (dto.IdProveedor.HasValue && !await _context.Proveedores.AnyAsync(p => p.IdProveedor == dto.IdProveedor.Value, cancellationToken))
                return BadRequest(new { mensaje = "El proveedor indicado no existe." });

            var numeroLote = dto.NumeroLote?.Trim();
            if (!string.IsNullOrWhiteSpace(numeroLote) &&
                await _context.Lotes.AnyAsync(l => l.NumeroLote == numeroLote, cancellationToken))
                return Conflict(new { mensaje = "Ya existe un lote con ese número." });

            var lote = new Lote
            {
                IdProducto = dto.IdProducto,
                NumeroLote = string.IsNullOrWhiteSpace(numeroLote) ? null : numeroLote,
                FechaElaboracion = dto.FechaElaboracion,
                FechaVencimiento = dto.FechaVencimiento,
                Tipo = dto.Tipo.Trim(),
                IdProveedor = dto.IdProveedor,
                EsReproceso = false
            };
            _context.Lotes.Add(lote);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(lote).Reference(l => l.Producto).LoadAsync(cancellationToken);
            await _context.Entry(lote).Reference(l => l.Proveedor).LoadAsync(cancellationToken);
            return CreatedAtAction(nameof(GetLote), new { id = lote.IdLote }, _mapper.Map<LoteDto>(lote));
        }
    }
}
