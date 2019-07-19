using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.Datos;
using Sistema.Entidades.Ventas;
using Sistema.Web.Models.Ventas.Personas;

namespace Sistema.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly DbContextSistema _context;

        public PersonasController(DbContextSistema context)
        {
            _context = context;
        }

        // GET: api/Personas
        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet]
        public IEnumerable<Persona> GetPersonas()
        {
            return _context.Personas;
        }

        // GET: api/Personas/ListarClientes
        [Authorize(Roles = "Vendedor, Administrador")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<PersonaViewModel>> ListarClientes()
        {
            var persona = await _context.Personas.Where(p => p.tipo_persona == "Cliente").ToListAsync();

            return persona.Select(p => new PersonaViewModel
            {
                idpersona = p.idpersona,
                tipo_persona = p.tipo_persona,
                nombre = p.nombre,
                tipo_documento = p.tipo_documento,
                num_documento = p.num_documento,
                direccion = p.direccion,
                telefono = p.telefono,
                email = p.email
            });
        }
        // GET: api/Personas/ListarProveedores
        [Authorize(Roles = "Almacenero, Administrador")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<PersonaViewModel>> ListarProveedores()
        {
            var persona = await _context.Personas.Where(p => p.tipo_persona == "Proveedor").ToListAsync();

            return persona.Select(p => new PersonaViewModel
            {
                idpersona = p.idpersona,
                tipo_persona = p.tipo_persona,
                nombre = p.nombre,
                tipo_documento = p.tipo_documento,
                num_documento = p.num_documento,
                direccion = p.direccion,
                telefono = p.telefono,
                email = p.email
            });
        }
        [Authorize(Roles = "Almacenero, Administrador")]
        // GET: api/Personas/SelectProveedores
        [HttpGet("[action]")]
        public async Task<IEnumerable<SelectViewModel>> SelectProveedores()
        {
            var persona = await _context.Personas.Where(c => c.tipo_persona == "Proveedor").ToListAsync();

            return persona.Select(p => new SelectViewModel
            {
                idpersona = p.idpersona,
                nombre = p.nombre
            });
        }
        [Authorize(Roles = "Vendedor, Administrador")]
        // GET: api/Personas/SelectClientes
        [HttpGet("[action]")]
        public async Task<IEnumerable<SelectViewModel>> SelectClientes()
        {
            var persona = await _context.Personas.Where(c => c.tipo_persona == "Cliente").ToListAsync();

            return persona.Select(p => new SelectViewModel
            {
                idpersona = p.idpersona,
                nombre = p.nombre
            });
        }

        // POST: api/Personas/Crear
        [Authorize(Roles = "Vendedor, Administrador,Almacenero")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Crear([FromBody] CrearViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = model.email.ToLower();

            if (await _context.Personas.AnyAsync(p => p.email == email))
            {
                return BadRequest("El email ya existe");
            }

            Persona persona = new Persona
            {
                
                tipo_persona = model.tipo_persona,
                nombre = model.nombre,
                tipo_documento= model.tipo_documento,
                num_documento = model.num_documento,
                direccion = model.direccion,
                telefono = model.telefono,
                email = model.email.ToLower()
            };

            _context.Personas.Add(persona);
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

        // PUT: api/Personas/Actualizar
        [Authorize(Roles = "Vendedor, Administrador,Almacenero")]
        [HttpPut("[action]")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarViewModel model)
        {
            //Se valida que el modelo que se recibe si este correcto
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Se valida que tenga algun dato el modelo
            if (model.idpersona <= 0)
            {
                return BadRequest();
            }
            //Me devuelve el dato que coincide con mi dato
            var persona = await _context.Personas.FirstOrDefaultAsync(p => p.idpersona == model.idpersona);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (persona == null)
            {
                return NotFound();
            }
            persona.tipo_persona = model.tipo_persona;
            persona.nombre = model.nombre;
            persona.tipo_documento = model.tipo_documento;
            persona.num_documento = model.num_documento;
            persona.direccion = model.direccion;
            persona.telefono = model.telefono;
            persona.email = model.email.ToLower();

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

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.idpersona == id);
        }
    }
}