using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NetCoreApi.Data;
using NetCoreApi.Models;

namespace NetCoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsuariosController(AppDbContext context)
    { 
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel usuario)
    {
        /*if(usuario.Contrasenia == "" || usuario.Contrasenia == null)
        {
            return BadRequest(new {message = "Ingrese la contraseña"});
        }
        if(usuario.Email == "" || usuario.Email == null)
        {
            return BadRequest(new {message = "Ingrese el usuario o email"});
        }*/
        if (string.IsNullOrWhiteSpace(usuario.Contrasenia) || string.IsNullOrWhiteSpace(usuario.Email))
        {
            return BadRequest(new {message = "El email y la contraseña son requeridos"});
        }
        var us = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == usuario.Email);
        if (us == null)
        {
            return Unauthorized(new {message = "Credenciales Ivalidas"});
        }
        if (usuario.Contrasenia != us.Contrasenia)
        {
            return Unauthorized(new {message = "Credenciales Ivalidas"});
        }
        return Ok(new
        {
            id = us.id,
            nombre = us.Nombres,
            email = us.Email
        });

    }

}