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
    [Authorize]
    public class ClientesController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/Clientes
        public IEnumerable<object> GetClientes()
        {
            return db.Clientes.OrderBy(c => c.Nombre).ToList().Select(c => ConvertirCliente(c)).AsEnumerable();
        }

        // GET: api/Clientes/5
        [ResponseType(typeof(Clientes))]
        public IHttpActionResult GetClientes(int id)
        {
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return NotFound();
            }

            return Ok(clientes);
        }

        [HttpGet]
        public bool Existe(string nombre)
        {
            return db.Clientes.Where(v => v.Nombre.Equals(nombre)).ToArray().Length > 0;
        }

        // PUT: api/Clientes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClientes(Clientes clientes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(clientes).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientesExists(clientes.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clientes
        [ResponseType(typeof(Clientes))]
        public IHttpActionResult PostClientes(Clientes clientes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Clientes.Add(clientes);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = clientes.Id }, ConvertirCliente(clientes));
        }

        // DELETE: api/Clientes/5
        [ResponseType(typeof(Clientes))]
        public IHttpActionResult DeleteClientes(int id)
        {
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return NotFound();
            }

            db.Clientes.Remove(clientes);
            db.SaveChanges();

            return Ok(clientes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientesExists(int id)
        {
            return db.Clientes.Count(e => e.Id == id) > 0;
        }

        private object ConvertirCliente(Clientes c)
        {
            return new
            {
                Direccion = c.Direccion,
                Id = c.Id,
                Nombre = c.Nombre,
                RNC = c.RNC,
                Telefono = c.Telefono,
                TieneCredito = c.TieneCredito,
                NombreCompleto = c.RNC+" "+c.Nombre
            };
        }
    }
}