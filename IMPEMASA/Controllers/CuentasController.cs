using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace IMPEMASA.Controllers
{
    [Authorize]
    public class CuentasController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/Cuentas
        public System.Collections.Generic.IEnumerable<object> GetCuentas()
        {
            return db.Cuentas.AsEnumerable().ToList().Select(c =>
            ConvertirCuenta(c)
            );
        }

        // GET: api/Cuentas/5
        [ResponseType(typeof(Cuentas))]
        public IHttpActionResult GetCuentas(int id)
        {
            Cuentas cuentas = db.Cuentas.Find(id);
            if (cuentas == null)
            {
                return NotFound();
            }

            return Ok(cuentas);
        }

        // PUT: api/Cuentas/5
        [ResponseType(typeof(Cuentas))]
        public IHttpActionResult PutCuentas(Cuentas cuentas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(cuentas).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentasExists(cuentas.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ConvertirCuenta(db.Cuentas.Include(c => c.Bancos).FirstOrDefault(c => c.Id.Equals(cuentas.Id))));
        }

        // POST: api/Cuentas
        [ResponseType(typeof(Cuentas))]
        public IHttpActionResult PostCuentas(Cuentas cuentas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Cuentas.Add(cuentas);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = cuentas.Id }, ConvertirCuenta(db.Cuentas.Include(c => c.Bancos).FirstOrDefault(c => c.Id.Equals(cuentas.Id))));
        }

        // DELETE: api/Cuentas/5
        [ResponseType(typeof(Cuentas))]
        public IHttpActionResult DeleteCuentas(int id)
        {
            Cuentas cuentas = db.Cuentas.Find(id);
            if (cuentas == null)
            {
                return NotFound();
            }

            db.Cuentas.Remove(cuentas);
            db.SaveChanges();

            return Ok(cuentas);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CuentasExists(int id)
        {
            return db.Cuentas.Count(e => e.Id == id) > 0;
        }

        private object ConvertirCuenta(Cuentas c)
        {
            return new
            {
                Activa = c.Activa,
                Id = c.Id,
                IdBanco = c.IdBanco,
                Numero = c.Numero,
                BancoAb = c.Bancos.Abreviatura
            };
        }
    }
}