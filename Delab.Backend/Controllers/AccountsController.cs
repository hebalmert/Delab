using Delab.AccessData.Data;
using Delab.Helpers;
using Delab.Shared.Entities;
using Delab.Shared.Enum;
using Delab.Shared.ResponsesSec;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Delab.Backend.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;
    private readonly IConfiguration _configuration;

    public AccountsController(DataContext context, IUserHelper userHelper,
        IConfiguration configuration)
    {
        _context = context;
        _userHelper = userHelper;
        _configuration = configuration;
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO modelo)
    {
        //TODO: Cambio de Path para Imagenes
        string imgUsuario = string.Empty;
        string ImagenDefault = $"https://localhost:7204/Images/NoImage.png";
        string BaseUrl = $"https://localhost:7204/Images/";

        var result = await _userHelper.LoginAsync(modelo);
        if (result.Succeeded)
        {
            //Consulto User de IdentityUser
            var user = await _userHelper.GetUserAsync(modelo.Email);
            if (!user.Active)
            {
                return BadRequest("El Usuario se Encuentra Inactivo, contacte al Administrador del Sistema");
            }
            var RolesUsuario = _context.UserRoleDetails.Where(c => c.UserId == user.Id).ToList();
            if (RolesUsuario.Count == 0)
            {
                return BadRequest("Este Usuario esta activo pero no tiene ningun Role Asignado...");
            }
            var RolUsuario = RolesUsuario.FirstOrDefault(c => c.UserType == UserType.Admin);
            if (RolUsuario == null)
            {
                var CheckCorporation = await _context.Corporations.FirstOrDefaultAsync(x => x.CorporationId == user.CorporationId);
                DateTime hoy = DateTime.Today;
                DateTime current = CheckCorporation!.DateEnd;
                if (!CheckCorporation.Active)
                {
                    return BadRequest("La Corporacion que trata de Acceder se encuentra Inactiva, Contacte al Administrador del Sistema");
                }
                if (current <= hoy)
                {
                    return BadRequest("El Tiempo del plan se ha cumplido, debe renovar su cuenta");
                }

                switch (user.UserFrom)
                {
                    case "Manager":
                        imgUsuario = user.PhotoUser != null ? $"{BaseUrl}ImgManager/{user.PhotoUser}" : ImagenDefault;
                        break;

                    case "UsuarioSoftware":
                        imgUsuario = user.PhotoUser != null ? $"{BaseUrl}ImgUsuarios/{user.PhotoUser}" : ImagenDefault;
                        break;
                }
            }
            return Ok(BuildToken(user, imgUsuario!));
        }

        if (result.IsLockedOut)
        {
            return BadRequest("Se Encuentra temporalmente Bloqueado");
        }

        if (result.IsNotAllowed)
        {
            return BadRequest("Lo Siento, No Tiene Acceso al Sistema");
        }

        return BadRequest("Usuario o Clave Erroneos");
    }

    [HttpPost("changePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }

        return NoContent();
    }

    private TokenDTO BuildToken(User user, string imgUsuario)
    {
        string NomCompa;
        string LogoCompa;
        var RolesUsuario = _context.UserRoleDetails.Where(c => c.UserId == user.Id).ToList();
        var RolUsuario = RolesUsuario.Where(c => c.UserType == UserType.Admin).FirstOrDefault();
        if (RolUsuario != null)
        {
            //TODO: Cambio de Path para Imagenes
            NomCompa = "Nexxplanet LLC";
            LogoCompa = $"https://localhost:7204/Images/NexxtplanetLLC.png";
            imgUsuario = $"https://localhost:7204/Images/NexxtplanetLLC.png";
        }
        else
        {
            var compname = _context.Corporations.FirstOrDefault(x => x.CorporationId == user.CorporationId);
            NomCompa = compname!.Name!;
            LogoCompa = compname!.ImageFullPath;
        }
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Photo", imgUsuario),
                new Claim("CorpName", NomCompa),
                new Claim("LogoCorp", LogoCompa),
            };
        // Agregar los roles del usuario a los claims
        foreach (var item in RolesUsuario)
        {
            claims.Add(new Claim(ClaimTypes.Role, item.UserType.ToString()!));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(3);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}