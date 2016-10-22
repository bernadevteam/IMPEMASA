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
    public class BancosController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/Bancos
        public IQueryable<object> GetBancos()
        {
            return db.Bancos.Select(b => new { Abreviatura = b.Abreviatura, Id = b.Id, Nombre = b.Nombre, Telefono = b.Telefono});
        }

        // GET: api/Bancos/5
        [ResponseType(typeof(Bancos))]
        public IHttpActionResult GetBancos(int id)
        {
            Bancos bancos = db.Bancos.Find(id);
            if (bancos == null)
            {
                return NotFound();
            }

            return Ok(bancos);
        }

        // PUT: api/Bancos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBancos(Bancos bancos)
        {

            db.Entry(bancos).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BancosExists(bancos.Id))
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

        // POST: api/Bancos
        [ResponseType(typeof(Bancos))]
        public IHttpActionResult PostBancos(Bancos bancos)
        {
            db.Bancos.Add(bancos);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bancos.Id }, bancos);
        }

        // DELETE: api/Bancos/5
        [ResponseType(typeof(Bancos))]
        public IHttpActionResult DeleteBancos(int id)
        {
            Bancos bancos = db.Bancos.Find(id);
            if (bancos == null)
            {
                return NotFound();
            }

            db.Bancos.Remove(bancos);
            db.SaveChanges();

            return Ok(bancos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BancosExists(int id)
        {
            return db.Bancos.Count(e => e.Id == id) > 0;
        }
    }
}