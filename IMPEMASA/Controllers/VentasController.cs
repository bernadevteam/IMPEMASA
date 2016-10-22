using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using IMPEMASA;

namespace IMPEMASA.Controllers
{
    public class VentasController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/Ventas
        public IEnumerable<object> GetVentas()
        {
            return db.Ventas.ToList().Select(v => ConvertirVenta(v));
        }

        // GET: api/Ventas/5
        [ResponseType(typeof(Ventas))]
        public IHttpActionResult GetVentas(int id)
        {
            Ventas ventas = db.Ventas.Find(id);
            if (ventas == null)
            {
                return NotFound();
            }

            return Ok(ventas);
        }

        // PUT: api/Ventas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVentas(Ventas ventas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(ventas).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentasExists(ventas.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ConvertirVenta(db.Ventas.
                Include(v => v.VentaTipos)
                .Include(v => v.Clientes)
                .First(v => ventas.Id.Equals(v.Id))));
        }

        // POST: api/Ventas
        [ResponseType(typeof(Ventas))]
        public IHttpActionResult PostVentas(Ventas ventas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ventas.Add(ventas);
            db.SaveChanges();

            ventas.Clientes = db.Clientes.Find(ventas.IdCliente);
            ventas.VentaTipos = db.VentaTipos.Find(ventas.IdVentaTipo);

            return CreatedAtRoute("DefaultApi", new { id = ventas.Id }, ConvertirVenta(ventas));
        }

        // DELETE: api/Ventas/5
        [ResponseType(typeof(Ventas))]
        public IHttpActionResult DeleteVentas(int id)
        {
            Ventas ventas = db.Ventas.Find(id);
            if (ventas == null)
            {
                return NotFound();
            }

            db.Ventas.Remove(ventas);
            db.SaveChanges();

            return Ok(ventas);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VentasExists(int id)
        {
            return db.Ventas.Count(e => e.Id == id) > 0;
        }

        private object ConvertirVenta(Ventas v)
        {
            return new
            {
                Id = v.Id,
                IdVentaTipo = v.IdVentaTipo,
                IdCliente = v.IdCliente,
                Fecha = v.Fecha.ToString("MM/dd/yyyy"),
                ITBIS = v.ITBIS,
                NoFactura = v.NoFactura,
                PagoPendiente = v.PagoPendiente,
                RNC = v.RNC,
                SubTotal = v.SubTotal,
                Total = v.Total,
                Cliente = v.Clientes.Nombre,
                Tipo = v.VentaTipos.Nombre
            };
        }
    }
}