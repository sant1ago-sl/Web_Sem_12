using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Data;
using TallerMecanico.Api.Models;

namespace TallerMecanico.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReparacionesController : ControllerBase
{
    private readonly TallerContext _db;
    public ReparacionesController(TallerContext db) => _db = db;

    // GET api/reparaciones?estado=&vehiculoId=&costoMin=&costoMax=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reparacion>>> Get(string? estado, int? vehiculoId, decimal? costoMin, decimal? costoMax)
    {
        var q = _db.Reparaciones.AsNoTracking().Include(r => r.Vehiculo).AsQueryable();
        if (!string.IsNullOrWhiteSpace(estado)) q = q.Where(r => r.Estado == estado);
        if (vehiculoId.HasValue) q = q.Where(r => r.VehiculoId == vehiculoId);
        if (costoMin.HasValue) q = q.Where(r => r.Costo >= costoMin);
        if (costoMax.HasValue) q = q.Where(r => r.Costo <= costoMax);
        return Ok(await q.OrderByDescending(r => r.Fecha).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Reparacion>> GetById(int id)
    {
        var r = await _db.Reparaciones.AsNoTracking().Include(x => x.Vehiculo).FirstOrDefaultAsync(x => x.Id == id);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpPost]
    public async Task<ActionResult<Reparacion>> Post(Reparacion reparacion)
    {
        reparacion.Id = 0;
        reparacion.Vehiculo = null;
        if (!await _db.Vehiculos.AnyAsync(v => v.Id == reparacion.VehiculoId))
            return BadRequest(new { mensaje = $"El vehiculo {reparacion.VehiculoId} no existe." });
        if (reparacion.Fecha == default) reparacion.Fecha = DateOnly.FromDateTime(DateTime.Today);

        _db.Reparaciones.Add(reparacion);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = reparacion.Id }, reparacion);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Reparacion reparacion)
    {
        if (id != reparacion.Id) return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo." });
        reparacion.Vehiculo = null;

        var actual = await _db.Reparaciones.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (actual is null) return NotFound();
        if (!await _db.Vehiculos.AnyAsync(v => v.Id == reparacion.VehiculoId))
            return BadRequest(new { mensaje = $"El vehiculo {reparacion.VehiculoId} no existe." });
        // Regla de negocio 409: una reparacion finalizada no se modifica
        if (actual.Estado == "Finalizada")
            return Conflict(new { mensaje = "Una reparacion finalizada no puede modificarse." });

        _db.Entry(reparacion).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _db.Reparaciones.FindAsync(id);
        if (r is null) return NotFound();
        // Regla de negocio 409: no eliminar reparaciones en proceso
        if (r.Estado == "EnProceso")
            return Conflict(new { mensaje = "No se puede eliminar una reparacion en proceso." });

        _db.Reparaciones.Remove(r);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
