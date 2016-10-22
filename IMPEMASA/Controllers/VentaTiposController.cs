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
    public class VentaTiposController : ApiController
    {
        private IMPEMASAEntities db = new IMPEMASAEntities();

        // GET: api/VentaTipos
        public IQueryable<object> GetVentaTipos()
        {
            return db.VentaTipos.Select(vt => new { Id = vt.Id, vt.Nombre });
        }

        // GET: api/VentaTipos/5
        [ResponseType(typeof(VentaTipos))]
        public IHttpActionResult GetVentaTipos(int id)
        {
            VentaTipos ventaTipos = db.VentaTipos.Find(id);
            if (ventaTipos == null)
            {
                return NotFound();
            }

            return Ok(ventaTipos);
        }

        // PUT: api/VentaTipos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVentaTipos(int id, VentaTipos ventaTipos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ventaTipos.Id)
            {
                return BadRequest();
            }

            db.Entry(ventaTipos).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentaTiposExists(id))
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

        // POST: api/VentaTipos
        [ResponseType(typeof(VentaTipos))]
        public IHttpActionResult PostVentaTipos(VentaTipos ventaTipos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.VentaTipos.Add(ventaTipos);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ventaTipos.Id }, ventaTipos);
        }

        // DELETE: api/VentaTipos/5
        [ResponseType(typeof(VentaTipos))]
        public IHttpActionResult DeleteVentaTipos(int id)
        {
            VentaTipos ventaTipos = db.VentaTipos.Find(id);
            if (ventaTipos == null)
            {
                return NotFound();
            }

            db.VentaTipos.Remove(ventaTipos);
            db.SaveChanges();

            return Ok(ventaTipos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VentaTiposExists(int id)
        {
            return db.VentaTipos.Count(e => e.Id == id) > 0;
        }
    }
}