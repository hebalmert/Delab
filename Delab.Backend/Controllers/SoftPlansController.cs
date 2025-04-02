using Delab.AccessData.Data;
using Delab.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delab.Backend.Controllers;

[Route("api/softplans")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class SoftPlansController : ControllerBase
{
    private readonly DataContext _context;

    public SoftPlansController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SoftPlan>> GetOneAsync(int id)
    {
        try
        {
            var modelo = await _context.SoftPlans.FindAsync(id);
            if (modelo == null)
            {
                return BadRequest("Problemas para conseguir el registro");
            }
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(SoftPlan modelo)
    {
        try
        {
            _context.SoftPlans.Update(modelo);
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

    [HttpPost]
    public async Task<ActionResult<SoftPlan>> PostAsync(SoftPlan modelo)
    {
        try
        {
            _context.SoftPlans.Add(modelo);
            await _context.SaveChangesAsync();
            return Ok(modelo);
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
    public async Task<IActionResult> DeleteAAsync(int id)
    {
        try
        {
            var DataRemove = await _context.SoftPlans.FindAsync(id);
            if (DataRemove == null)
            {
                return BadRequest("Problemas para conseguir el registro");
            }
            _context.SoftPlans.Remove(DataRemove);
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