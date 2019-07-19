using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sistema.Datos;
using Sistema.Entidades.Usuarios;
using Sistema.Web.Models.Usuarios.Usuario;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DbContextSistema _context;
        private readonly IConfiguration _config;

        public UsuariosController(DbContextSistema context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Usuarios/Listar
        [Authorize(Roles = "Administrador")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<UsuarioViewModel>> Listar()
        {
            var usuario = await _context.Usuarios.Include(a => a.rol).ToListAsync();

            return usuario.Select(u => new UsuarioViewModel
            {
                idusuario = u.idusuario,
                idrol = u.idrol,
                rol = u.rol.nombre,
                nombre = u.nombre,
                tipo_documento = u.tipo_documento,
                num_documento = u.num_documento,
                direccion = u.direccion,
                telefono = u.telefono,
                email = u.email,
                password_hash = u.password_hash,
                condicion = u.condicion
            });
        }

        // POST: api/Usuarios/Crear
        [HttpPost("[action]")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] CrearViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = model.email.ToLower();

            if (await _context.Usuarios.AnyAsync(u => u.email == email))
            {
                return BadRequest("El email ya existe");
            }
            //Metodo de contraseña
            CrearPasswordHas(model.password, out byte[] passwordHash, out byte[] passwordSalt);

            Usuario usuario = new Usuario
            {
                idrol = model.idrol,
                nombre = model.nombre,
                tipo_documento = model.tipo_documento,
                num_documento = model.num_documento,
                direccion = model.direccion,
                telefono = model.telefono,
                email = model.email.ToLower(),
                password_hash = passwordHash,
                password_salt = passwordSalt,
                condicion = true
            };

            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

        // PUT: api/Usuarios/Actualizar
        [HttpPut("[action]")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarViewModel model)
        {
            //Se valida que el modelo que se recibe si este correcto
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Se valida que tenga algun dato el modelo
            if (model.idusuario <= 0)
            {
                return BadRequest();
            }
            //Me devuelve el dato que coincide con mi dato
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(a => a.idusuario == model.idusuario);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.idrol = model.idrol;
            usuario.nombre = model.nombre;
            usuario.tipo_documento = model.tipo_documento;
            usuario.num_documento = model.num_documento;
            usuario.direccion = model.direccion;
            usuario.telefono = model.telefono;
            usuario.email = model.email.ToLower();

            if (model.act_password == true)
            {
                CrearPasswordHas(model.password, out byte[] passwordHash, out byte[] passwordSalt);
                usuario.password_hash = passwordHash;
                usuario.password_salt = passwordSalt;
            }


            try
            {//Se guardan los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return Ok();
        }

        // PUT: api/Usuarios/Desactivar/1
        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Desactivar([FromRoute] int id)
        {
            //Se valida que el modelo que se recibe si este correcto
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Se valida que tenga algun dato el modelo
            if (id <= 0)
            {
                return BadRequest();
            }
            //Me devuelve el dato que coincide con mi dato
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(c => c.idusuario == id);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.condicion = false;

            try
            {//Se guardan los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return Ok();
        }

        // PUT: api/Usuarios/Activar/1
        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Activar([FromRoute] int id)
        {
            //Se valida que el modelo que se recibe si este correcto
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Se valida que tenga algun dato el modelo
            if (id <= 0)
            {
                return BadRequest();
            }
            //Me devuelve el dato que coincide con mi dato
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(c => c.idusuario == id);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.condicion = true;

            try
            {//Se guardan los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }

            return Ok();
        }

        // POST: api/Usuarios/Login
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var email = model.email.ToLower();

            var usuario = await _context.Usuarios.Where( u => u.condicion==true).Include(u => u.rol).FirstOrDefaultAsync(u => u.email == email);

            if( usuario == null)
            {
                return NotFound();
            }

            if (!VerificarPasswordHash(model.password,usuario.password_hash,usuario.password_salt))
            {
                return NotFound();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.idusuario.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, usuario.rol.nombre),
                new Claim("idusuario", usuario.idusuario.ToString()),
                new Claim("rol", usuario.rol.nombre),
                new Claim("nombre", usuario.nombre)
            };

            return Ok(
                new { token = GenerarToken(claims)}
                );

        }

        private string GenerarToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds,
                claims: claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private bool VerificarPasswordHash(string password, byte [] passwordHashAlmacenado, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var passwordHashNuevo = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return new ReadOnlySpan<byte>(passwordHashAlmacenado).SequenceEqual(new ReadOnlySpan<byte>(passwordHashNuevo));
            }
        }

        //Metodo para encríptar la contraseña
        private void CrearPasswordHas(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.idusuario == id);
        }
    }
}