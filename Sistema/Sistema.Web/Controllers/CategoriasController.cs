using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.Datos;
using Sistema.Entidades.Almacen;
using Sistema.Web.Models.Almacen.Categoria;

namespace Sistema.Web.Controllers
{
    [Authorize(Roles = "Almacenero, Administrador")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly DbContextSistema _context;

        public CategoriasController(DbContextSistema context)
        {
            _context = context;
        }

        // GET: api/Categorias/Listar
        [HttpGet("[action]")]
        public  async Task <IEnumerable<CategoriaViewModel>> Listar()
        {
            var categoria = await _context.Categorias.ToListAsync();

            return categoria.Select(c => new CategoriaViewModel
            {
                idcategoria = c.idcategoria,
                nombre = c.nombre,
                descripcion = c.descripcion,
                condicion = c.condicion
            });
        }
        // GET: api/Categorias/Select
        [HttpGet("[action]")]
        public async Task<IEnumerable<SelectViewModel>> Select()
        {
            var categoria = await _context.Categorias.Where(c => c.condicion==true).ToListAsync();

            return categoria.Select(c => new SelectViewModel
            {
                idcategoria = c.idcategoria,
                nombre = c.nombre
            });
        }

        // GET: api/Categorias/Mostrar/1
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Mostrar([FromRoute] int id)
        {

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return Ok(new CategoriaViewModel {
                idcategoria = categoria.idcategoria,
                nombre = categoria.nombre,
                descripcion = categoria.descripcion,
                condicion = categoria.condicion
            });
        }

        // PUT: api/Categorias/Actualizar
        [HttpPut("[action]")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarViewModel model)
        {
            //Se valida que el modelo que se recibe si este correcto
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Se valida que tenga algun dato el modelo
            if (model.idcategoria <= 0 )
            {
                return BadRequest();
            }
            //Me devuelve el dato que coincide con mi dato
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idcategoria == model.idcategoria);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if(categoria == null)
            {
                return NotFound();
            }
           
            categoria.nombre = model.nombre;
            categoria.descripcion = model.descripcion;

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

        // POST: api/Categorias/Crear
        [HttpPost("[action]")]
        public async Task<IActionResult> Crear([FromBody] CrearViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Categoria categoria = new Categoria
            {
                nombre = model.nombre,
                descripcion = model.descripcion,
                condicion = true
            };

            _context.Categorias.Add(categoria);
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

        // DELETE: api/Categorias/Eliminar/5
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Eliminar([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoria);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }

            return Ok(categoria);
        }

        // PUT: api/Categorias/Desactivar/1
        [HttpPut("[action]/{id}")]
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
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idcategoria == id);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (categoria == null)
            {
                return NotFound();
            }
            categoria.condicion = false;

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

        // PUT: api/Categorias/Activar/1
        [HttpPut("[action]/{id}")]
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
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idcategoria == id);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (categoria == null)
            {
                return NotFound();
            }
            categoria.condicion = true;

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
    }
}