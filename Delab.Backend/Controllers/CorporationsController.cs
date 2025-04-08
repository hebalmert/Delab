using Delab.AccessData.Data;
using Delab.Backend.helpers;
using Delab.Helpers;
using Delab.Shared.Entities;
using Delab.Shared.Pagination;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delab.Backend.Controllers;

[Route("api/corporations")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class CorporationsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly string ImgRoute;

    public CorporationsController(DataContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
        //TODO: Cambio de ruta para Imagenes
        ImgRoute = "wwwroot\\Images\\ImgCorporation";
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Corporation>>> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.Corporations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Corporation>> GetOneAsync(int id)
    {
        try
        {
            var modelo = await _context.Corporations.FindAsync(id);
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
    public async Task<ActionResult> PutAsync(Corporation modelo)
    {
        try
        {
            if (!string.IsNullOrEmpty(modelo.ImgBase64))
            {
                string guid;
                if (modelo.Imagen == null)
                {
                    guid = Guid.NewGuid().ToString() + ".jpg";
                }
                else
                {
                    guid = modelo.Imagen;
                }
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.Imagen = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Corporations.Update(modelo);
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
    public async Task<ActionResult<Corporation>> PostAsync(Corporation modelo)
    {
        try
        {
            if (!string.IsNullOrEmpty(modelo.ImgBase64))
            {
                string guid = Guid.NewGuid().ToString() + ".jpg";
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.Imagen = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Corporations.Add(modelo);
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
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var DataRemove = await _context.Corporations.FindAsync(id);
            if (DataRemove == null)
            {
                return BadRequest("Problemas para conseguir el registro");
            }
            _context.Corporations.Remove(DataRemove);
            await _context.SaveChangesAsync();

            if (DataRemove.Imagen is not null)
            {
                var response = _fileStorage.DeleteImage(ImgRoute, DataRemove.Imagen);
                if (!response)
                {
                    return BadRequest("Se Elimino el Registro pero sin la Imagen");
                }
            }

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