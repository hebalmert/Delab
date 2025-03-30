using Delab.AccessData.Data;
using Delab.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delab.Backend.Controllers;

[Route("api/countries")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly DataContext _context;

    public CountriesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
    {
        try
        {
            var listCountry = await _context.Countries
                .Include(X => X.States)!.ThenInclude(x => x.Cities).OrderBy(x => x.Name).ToListAsync();
            return Ok(listCountry);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
        try
        {
            var CountryName = await _context.Countries.FindAsync(id);
            return Ok(CountryName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostCountry([FromBody] Country modelo)
    {
        try
        {
            _context.Countries.Add(modelo);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException!.Message.Contains("duplicate"))
            {
                return BadRequest("Ya Existe un Registro con el mismo nombre.");
            }
            else
            {
                return BadRequest(dbEx.InnerException.Message);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<Country>> PutCountry(Country modelo)
    {
        try
        {
            var UpdateCountry = await _context.Countries.FirstOrDefaultAsync(x => x.CountryId == modelo.CountryId);
            UpdateCountry!.Name = modelo.Name;
            UpdateCountry.CodPhone = modelo.CodPhone;

            _context.Countries.Update(UpdateCountry);
            await _context.SaveChangesAsync();

            return Ok(UpdateCountry);
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException!.Message.Contains("duplicate"))
            {
                return BadRequest("Ya Existe un Registro con el mismo nombre.");
            }
            else
            {
                return BadRequest(dbEx.InnerException.Message);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        try
        {
            var DataRemove = await _context.Countries.FindAsync(id);
            if (DataRemove == null)
            {
                return BadRequest("No se Encontro el registro para Borrar");
            }
            _context.Countries.Remove(DataRemove);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException!.Message.Contains("REFERENCE"))
            {
                return BadRequest("No puede Eliminar el registro porque tiene datos Relacionados");
            }
            else
            {
                return BadRequest(dbEx.InnerException.Message);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}