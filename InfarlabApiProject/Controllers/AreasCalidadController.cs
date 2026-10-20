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
    public class AreasCalidadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AreasCalidadController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaCalidadDto>>> GetEvaluaciones(CancellationToken cancellationToken)
        {
            var evaluaciones = await _context.AreasCalidad.AsNoTracking()
                .Include(a => a.Lote)
                .Include(a => a.AlmacenDestino)
                .OrderByDescending(a => a.FechaEvaluacion)
                .ToListAsync(cancellationToken);
            return Ok(_mapper.Map<List<AreaCalidadDto>>(evaluaciones));
        }

        [HttpGet("lote/{idLote:int}")]
        public async Task<ActionResult<IEnumerable<AreaCalidadDto>>> GetEvaluacionesPorLote(int idLote, CancellationToken cancellationToken)
        {
            var evaluaciones = await _context.AreasCalidad.AsNoTracking()
                .Where(a => a.IdLote == idLote)
                .Include(a => a.Lote)
                .Include(a => a.AlmacenDestino)
                .OrderByDescending(a => a.FechaEvaluacion)
                .ToListAsync(cancellationToken);
            return Ok(_mapper.Map<List<AreaCalidadDto>>(evaluaciones));
        }

        [HttpPost]
        public async Task<ActionResult<AreaCalidadDto>> RegistrarEvaluacion([FromBody] CrearAreaCalidadDto dto, CancellationToken cancellationToken)
        {
            if (dto.FechaResultado.HasValue && dto.FechaResultado.Value < dto.FechaEvaluacion)
                return BadRequest(new { mensaje = "La fecha del resultado no puede ser anterior a la evaluación." });

            if (!await _context.Lotes.AnyAsync(l => l.IdLote == dto.IdLote, cancellationToken))
                return BadRequest(new { mensaje = "El lote indicado no existe." });
            if (!await _context.Almacenes.AnyAsync(a => a.IdAlmacen == dto.IdAlmacenDestino, cancellationToken))
                return BadRequest(new { mensaje = "El almacén de destino indicado no existe." });

            var evaluacion = new AreaCalidad
            {
                IdLote = dto.IdLote,
                IdAlmacenDestino = dto.IdAlmacenDestino,
                Descripcion = dto.Descripcion?.Trim(),
                FechaEvaluacion = dto.FechaEvaluacion,
                FechaResultado = dto.FechaResultado
            };
            _context.AreasCalidad.Add(evaluacion);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(evaluacion).Reference(a => a.Lote).LoadAsync(cancellationToken);
            await _context.Entry(evaluacion).Reference(a => a.AlmacenDestino).LoadAsync(cancellationToken);
            return CreatedAtAction(nameof(GetEvaluacionesPorLote), new { idLote = evaluacion.IdLote }, _mapper.Map<AreaCalidadDto>(evaluacion));
        }
    }
}
