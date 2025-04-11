using Delab.AccessData.Data;
using Delab.Backend.helpers;
using Delab.Helpers;
using Delab.Shared.Entities;
using Delab.Shared.Enum;
using Delab.Shared.Pagination;
using Delab.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delab.Backend.Controllers;

[Route("api/managers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
[ApiController]
public class ManagersController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;
    private readonly IFileStorage _fileStorage;
    private readonly IConfiguration _configuration;
    private readonly IEmailHelper _emailHelper;
    private readonly string ImgRoute;

    public ManagersController(DataContext context, IUserHelper userHelper, IFileStorage fileStorage,
        IConfiguration configuration, IEmailHelper emailHelper)
    {
        _context = context;
        _userHelper = userHelper;
        _fileStorage = fileStorage;
        _configuration = configuration;
        _emailHelper = emailHelper;
        ImgRoute = "wwwroot\\Images\\ImgManager";
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Manager>>> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.Managers.Include(x => x.Corporation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.FullName!.ToLower().Contains(pagination.Filter.ToLower()));
        }

        await HttpContext.InsertParameterPagination(queryable, pagination.RecordsNumber);
        return await queryable.OrderBy(x => x.FullName).Paginate(pagination).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Manager>> GetOneAsync(int id)
    {
        try
        {
            var modelo = await _context.Managers
        .Include(x => x.Corporation).FirstOrDefaultAsync(x => x.ManagerId == id);
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
    public async Task<IActionResult> PutAsync(Manager modelo)
    {
        try
        {
            Manager NewModelo = new()
            {
                ManagerId = modelo.ManagerId,
                FirstName = modelo.FirstName,
                LastName = modelo.LastName,
                FullName = $"{modelo.FirstName} {modelo.LastName}",
                Nro_Document = modelo.Nro_Document,
                PhoneNumber = modelo.PhoneNumber,
                Address = modelo.Address,
                UserName = modelo.UserName,
                CorporationId = modelo.CorporationId,
                Job = modelo.Job,
                UserType = modelo.UserType,
                Photo = modelo.Photo,
                Active = modelo.Active,
            };
            if (modelo.ImgBase64 != null)
            {
                NewModelo.ImgBase64 = modelo.ImgBase64;
            }

            //Respaldamos la base de datos antes de hacer operaciones
            var transaction = await _context.Database.BeginTransactionAsync();

            if (!string.IsNullOrEmpty(modelo.ImgBase64))
            {
                string guid;
                if (modelo.Photo == null)
                {
                    guid = Guid.NewGuid().ToString() + ".jpg";
                }
                else
                {
                    guid = modelo.Photo;
                }
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                NewModelo.Photo = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Managers.Update(NewModelo);
            await _context.SaveChangesAsync();

            User UserCurrent = await _userHelper.GetUserAsync(modelo.UserName);
            if (UserCurrent != null)
            {
                UserCurrent.FirstName = modelo.FirstName;
                UserCurrent.LastName = modelo.LastName;
                UserCurrent.FullName = $"{modelo.FirstName} {modelo.LastName}";
                UserCurrent.PhoneNumber = modelo.PhoneNumber;
                UserCurrent.PhotoUser = modelo.Photo;
                UserCurrent.JobPosition = modelo.Job;
                UserCurrent.Active = modelo.Active;
                IdentityResult result = await _userHelper.UpdateUserAsync(UserCurrent);
            }
            else
            {
                if (modelo.Active)
                {
                    Response response = await AcivateUser(modelo);
                    if (response.IsSuccess == false)
                    {
                        var guid = modelo.Photo;
                        _fileStorage.DeleteImage(ImgRoute, guid!);
                        await transaction.RollbackAsync();
                        return BadRequest("No se ha podido crear el Usuario, Intentelo de nuevo");
                    }
                }
            }

            await transaction.CommitAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Manager>> PostAsync(Manager modelo)
    {
        try
        {
            User CheckEmail = await _userHelper.GetUserAsync(modelo.UserName);
            if (CheckEmail != null)
            {
                return BadRequest("El Correo ingresado ya se encuentra reservado, debe cambiarlo.");
            }

            //En Caso de Fallo Restaurar la Base de Datos
            var transction = await _context.Database.BeginTransactionAsync();

            modelo.FullName = $"{modelo.FirstName} {modelo.LastName}";
            modelo.UserType = UserType.Usuario;
            if (!string.IsNullOrEmpty(modelo.ImgBase64))
            {
                string guid = Guid.NewGuid().ToString() + ".jpg";
                var imageId = Convert.FromBase64String(modelo.ImgBase64);
                modelo.Photo = await _fileStorage.UploadImage(imageId, ImgRoute, guid);
            }
            _context.Managers.Add(modelo);
            await _context.SaveChangesAsync();

            //Registro del Usuario en User
            if (modelo.Active)
            {
                Response response = await AcivateUser(modelo);
                if (!response.IsSuccess)
                {
                    var guid = modelo.Photo;
                    _fileStorage.DeleteImage(ImgRoute, guid!);
                    await transction.RollbackAsync();
                    return BadRequest("No se ha podido crear el Usuario, Intentelo de nuevo");
                }
            }
            await transction.CommitAsync();
            return Ok(modelo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<Response> AcivateUser(Manager manager)
    {
        User user = await _userHelper.AddUserUsuarioAsync(manager.FirstName, manager.LastName, manager.UserName,
            manager.PhoneNumber, manager.Address, manager.Job, manager.CorporationId, manager.Photo!, "Manager", manager.Active, manager.UserType);

        //Envio de Correo con Token de seguridad para Verificar el correo
        string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
        string tokenLink = Url.Action("ConfirmEmail", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["UrlFrontend"])!.Replace("api/managers", "api/accounts");

        string subject = "Activacion de Cuenta";
        string body = ($"De: NexxtPlanet" +
            $"<h1>Email Confirmation</h1>" +
            $"<p>" +
            $"Su Clave Temporal es: <h2> \"{user.Pass}\"</h2>" +
            $"</p>" +
            $"Para Activar su vuenta, " +
            $"Has Click en el siguiente Link:</br></br><strong><a href = \"{tokenLink}\">Confirmar Correo</a></strong>");

        Response response = await _emailHelper.ConfirmarCuenta(user.UserName!, user.FullName!, subject, body);
        if (response.IsSuccess == false)
        {
            return response;
        }
        return response;
    }
}