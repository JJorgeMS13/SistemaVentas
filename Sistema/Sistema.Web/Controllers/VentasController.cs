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
using Sistema.Web.Models.Ventas.Venta;

namespace Sistema.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly DbContextSistema _context;

        public VentasController(DbContextSistema context)
        {
            _context = context;
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        // GET: api/Ventas/Listar
        [HttpGet("[action]")]
        public async Task<IEnumerable<VentaViewModel>> Listar()
        {
            var venta = await _context.Ventas
                .Include(v => v.usuario)
                .Include(p => p.persona)
                .OrderByDescending(i => i.idventa)
                .Take(100).ToListAsync();

            return venta.Select(v => new VentaViewModel   
            {
                idventa = v.idventa,
                idcliente = v.idcliente,
                cliente = v.persona.nombre,
                num_documento = v.persona.num_documento,
                direccion = v.persona.direccion,
                telefono = v.persona.telefono,
                email = v.persona.email,
                idusuario = v.idusuario,
                usuario = v.usuario.nombre,
                tipo_comprobante = v.tipo_comprobante,
                serie_comprobante = v.serie_comprobante,
                num_comprobante = v.num_comprobante,
                fecha_hora = v.fecha_hora,
                impuesto = v.impuesto,
                total = v.total,
                estado = v.estado

            });
        }

        [Authorize(Roles = "Vendedor, Administrador, Almacenero")]
        // GET: api/Ventas/VentasMes12
        [HttpGet("[action]")]
        public async Task<IEnumerable<ConsultaViewModel>> VentasMes12()
        {
            var consulta = await _context.Ventas
                .GroupBy(v => v.fecha_hora.Month)
                .Select(x => new { etiqueta= x.Key, valor=x.Sum(v => v.total)})
                .OrderByDescending(i => i.etiqueta)
                .Take(12).ToListAsync();

            return consulta.Select(v => new ConsultaViewModel
            {
                etiqueta = v.etiqueta.ToString(),
                valor = v.valor
            });
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        // GET: api/Ventas/ConsultaFechas/FechaInicio/FechaFin
        [HttpGet("[action]/{FechaInicio}/{FechaFin}")]
        public async Task<IEnumerable<VentaViewModel>> ConsultaFechas([FromRoute] DateTime FechaInicio, DateTime FechaFin)
        {
            var venta = await _context.Ventas
                .Include(v => v.usuario)
                .Include(p => p.persona)
                .Where(i => i.fecha_hora >= FechaInicio)
                .Where(i => i.fecha_hora <= FechaFin)
                .OrderByDescending(i => i.idventa)
                .Take(100).ToListAsync();

            return venta.Select(v => new VentaViewModel
            {
                idventa = v.idventa,
                idcliente = v.idcliente,
                cliente = v.persona.nombre,
                num_documento = v.persona.num_documento,
                direccion = v.persona.direccion,
                telefono = v.persona.telefono,
                email = v.persona.email,
                idusuario = v.idusuario,
                usuario = v.usuario.nombre,
                tipo_comprobante = v.tipo_comprobante,
                serie_comprobante = v.serie_comprobante,
                num_comprobante = v.num_comprobante,
                fecha_hora = v.fecha_hora,
                impuesto = v.impuesto,
                total = v.total,
                estado = v.estado

            });
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        // GET: api/Ventas/Listar
        [HttpGet("[action]/{texto}")]
        public async Task<IEnumerable<VentaViewModel>> ListarFiltro([FromRoute] string texto)
        {
            var venta = await _context.Ventas
                .Include(v => v.usuario)
                .Include(p => p.persona)
                .Where(v => v.num_comprobante.Contains(texto))
                .OrderByDescending(v => v.idventa)
                .ToListAsync();

            return venta.Select(v => new VentaViewModel
            {
                idventa = v.idventa,
                idcliente = v.idcliente,
                cliente = v.persona.nombre,
                idusuario = v.idusuario,
                usuario = v.usuario.nombre,
                num_documento = v.persona.num_documento,
                direccion = v.persona.direccion,
                telefono = v.persona.telefono,
                email = v.persona.email,
                tipo_comprobante = v.tipo_comprobante,
                serie_comprobante = v.serie_comprobante,
                num_comprobante = v.num_comprobante,
                fecha_hora = v.fecha_hora,
                impuesto = v.impuesto,
                total = v.total,
                estado = v.estado

            });
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        // GET: api/Ventas/ListarDetalles
        [HttpGet("[action]/{idventa}")]
        public async Task<IEnumerable<DetalleViewModel>> ListarDetalles([FromRoute] int idventa)
        {
            var detalle = await _context.DetalleVentas
                .Include(a => a.articulo)
                .Where(d => d.idventa == idventa)
                .ToListAsync();

            return detalle.Select(d => new DetalleViewModel
            {
                idarticulo = d.idarticulo,
                articulo = d.articulo.nombre,
                cantidad = d.cantidad,
                precio = d.precio,
                descuento = d.descuento

            });
        }


        [Authorize(Roles = "Vendedor, Administrador")]
        // POST: api/Ventas/Crear
        [HttpPost("[action]")]
        public async Task<IActionResult> Crear([FromBody] CrearViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var fechaHora = DateTime.Now;
            Venta venta = new Venta
            {
                idcliente = model.idcliente,
                idusuario = model.idusuario,
                tipo_comprobante = model.tipo_comprobante,
                serie_comprobante = model.serie_comprobante,
                num_comprobante = model.num_comprobante,
                fecha_hora = fechaHora,
                impuesto = model.impuesto,
                total = model.total,
                estado = "Aceptado"
            };


            try
            {
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();
                var id = venta.idventa;
                foreach (var det in model.detalles)
                {
                    DetalleVenta detalle = new DetalleVenta
                    {
                        idventa = id,
                        idarticulo = det.idarticulo,
                        cantidad = det.cantidad,
                        precio = det.precio,
                        descuento = det.descuento
                    };
                    _context.DetalleVentas.Add(detalle);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

        [Authorize(Roles = "Vendedor, Administrador")]
        // PUT: api/Ventas/Anular/1
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Anular([FromRoute] int id)
        {
            //Se valida que tenga algun dato el modelo
            if (id <= 0)
            {
                return BadRequest();
            }
            //Me devuelve el dato que coincide con mi dato
            var venta = await _context.Ventas.FirstOrDefaultAsync(c => c.idventa == id);
            //Se valida que el dato ingresado se igual alguno de la base de datos
            if (venta == null)
            {
                return NotFound();
            }
            venta.estado = "Anulado";

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
        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.idventa == id);
        }
    }
}