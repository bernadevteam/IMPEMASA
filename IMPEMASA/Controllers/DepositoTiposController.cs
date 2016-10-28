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
    public class DepositoTiposController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/DepositoTipos
        public IEnumerable<object> GetDepositoTipos()
        {
            return db.DepositoTipos.ToList().Select(dt => new DepositoTipos() { Id = dt.Id, Nombre = dt.Nombre});
        }

        // GET: api/DepositoTipos/5
        [ResponseType(typeof(DepositoTipos))]
        public IHttpActionResult GetDepositoTipos(int id)
        {
            DepositoTipos depositoTipos = db.DepositoTipos.Find(id);
            if (depositoTipos == null)
            {
                return NotFound();
            }

            return Ok(depositoTipos);
        }

        // PUT: api/DepositoTipos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDepositoTipos(int id, DepositoTipos depositoTipos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != depositoTipos.Id)
            {
                return BadRequest();
            }

            db.Entry(depositoTipos).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepositoTiposExists(id))
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

        // POST: api/DepositoTipos
        [ResponseType(typeof(DepositoTipos))]
        public IHttpActionResult PostDepositoTipos(DepositoTipos depositoTipos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DepositoTipos.Add(depositoTipos);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = depositoTipos.Id }, depositoTipos);
        }

        // DELETE: api/DepositoTipos/5
        [ResponseType(typeof(DepositoTipos))]
        public IHttpActionResult DeleteDepositoTipos(int id)
        {
            DepositoTipos depositoTipos = db.DepositoTipos.Find(id);
            if (depositoTipos == null)
            {
                return NotFound();
            }

            db.DepositoTipos.Remove(depositoTipos);
            db.SaveChanges();

            return Ok(depositoTipos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepositoTiposExists(int id)
        {
            return db.DepositoTipos.Count(e => e.Id == id) > 0;
        }
    }
}