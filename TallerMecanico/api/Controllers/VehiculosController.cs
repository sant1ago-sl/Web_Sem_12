using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Data;
using TallerMecanico.Api.Models;

namespace TallerMecanico.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculosController : ControllerBase
{
    private readonly TallerContext _db;
    public VehiculosController(TallerContext db) => _db = db;

    // GET api/vehiculos?marca=&propietario=&anioDesde=&anioHasta=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> Get(string? marca, string? propietario, int? anioDesde, int? anioHasta)
    {
        var q = _db.Vehiculos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(marca)) q = q.Where(v => v.Marca.Contains(marca));
        if (!string.IsNullOrWhiteSpace(propietario)) q = q.Where(v => v.Propietario.Contains(propietario));
        if (anioDesde.HasValue) q = q.Where(v => v.Anio >= anioDesde);
        if (anioHasta.HasValue) q = q.Where(v => v.Anio <= anioHasta);
        return Ok(await q.OrderBy(v => v.Placa).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Vehiculo>> GetById(int id)
    {
        var v = await _db.Vehiculos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return v is null ? NotFound() : Ok(v);
    }

    // Ruta anidada: GET api/vehiculos/{id}/reparaciones
    [HttpGet("{id:int}/reparaciones")]
    public async Task<ActionResult<IEnumerable<Reparacion>>> GetReparaciones(int id)
    {
        if (!await _db.Vehiculos.AnyAsync(v => v.Id == id)) return NotFound();
        var lista = await _db.Reparaciones.AsNoTracking()
            .Where(r => r.VehiculoId == id).OrderByDescending(r => r.Fecha).ToListAsync();
        return Ok(lista);
    }

    [HttpPost]
    public async Task<ActionResult<Vehiculo>> Post(Vehiculo vehiculo)
    {
        vehiculo.Id = 0;
        // Regla de negocio 409: la placa debe ser unica
        if (await _db.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa))
            return Conflict(new { mensaje = $"Ya existe un vehiculo con la placa {vehiculo.Placa}." });

        _db.Vehiculos.Add(vehiculo);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = vehiculo.Id }, vehiculo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Vehiculo vehiculo)
    {
        if (id != vehiculo.Id) return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo." });
        if (!await _db.Vehiculos.AnyAsync(v => v.Id == id)) return NotFound();
        if (await _db.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa && v.Id != id))
            return Conflict(new { mensaje = $"Ya existe otro vehiculo con la placa {vehiculo.Placa}." });

        _db.Entry(vehiculo).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var v = await _db.Vehiculos.FindAsync(id);
        if (v is null) return NotFound();
        // Regla de negocio 409: no eliminar vehiculos con reparaciones registradas
        if (await _db.Reparaciones.AnyAsync(r => r.VehiculoId == id))
            return Conflict(new { mensaje = "No se puede eliminar: el vehiculo tiene reparaciones registradas." });

        _db.Vehiculos.Remove(v);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
