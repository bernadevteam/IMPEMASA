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
    public class DepositosController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/Depositos
        public IEnumerable<object> GetDepositos()
        {
            return db.Depositos.ToList().Select(d => Convertir(d));
        }

        // GET: api/Depositos/5
        [ResponseType(typeof(Depositos))]
        public IHttpActionResult GetDepositos(int id)
        {
            Depositos depositos = db.Depositos.Find(id);
            if (depositos == null)
            {
                return NotFound();
            }

            return Ok(depositos);
        }

        // PUT: api/Depositos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDepositos(Depositos depositos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            db.Entry(depositos).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepositosExists(depositos.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(Convertir(db.Depositos.Include(d => d.Cuentas).Include(d => d.DepositoTipos).First(d => d.Id.Equals(depositos.Id))));
        }

        // POST: api/Depositos
        [ResponseType(typeof(Depositos))]
        public IHttpActionResult PostDepositos(Depositos depositos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Depositos.Add(depositos);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = depositos.Id }, Convertir(db.Depositos.Include(d => d.Cuentas).Include(d => d.DepositoTipos).First(d => d.Id.Equals(depositos.Id))));
        }

        // DELETE: api/Depositos/5
        [ResponseType(typeof(Depositos))]
        public IHttpActionResult DeleteDepositos(int id)
        {
            Depositos depositos = db.Depositos.Find(id);
            if (depositos == null)
            {
                return NotFound();
            }

            db.Depositos.Remove(depositos);
            db.SaveChanges();

            return Ok(depositos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepositosExists(int id)
        {
            return db.Depositos.Count(e => e.Id == id) > 0;
        }

        public static object Convertir(Depositos d)
        {
            return new
            {
                Id = d.Id,
                IdCuenta = d.IdCuenta,
                IdDepositoTipo = d.IdDepositoTipo,
                IdVenta = d.IdVenta,
                Cuenta = d.Cuentas.Numero,
                Banco = d.Cuentas.Bancos.Abreviatura,
                Tipo = d.DepositoTipos.Nombre,
                Monto = d.Monto,
                Fecha = d.Fecha.ToString("MM/dd/yyyy")
            };
        }
    }
}